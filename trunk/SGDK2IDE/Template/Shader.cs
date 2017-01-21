using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

/// <summary>
/// Encapsulates the information that we want to track for every tile corner drawn
/// </summary>
struct TileVertex
{
   public const int Size = 8 * 4; // size of struct in bytes

   private readonly Vector2 position;
   private readonly Vector2 source;
   private readonly Color4 color;

   public TileVertex(float x, float y, float srcX, float srcY)
      : this(x, y, srcX, srcY, Color4.White)
   {
   }

   public TileVertex(float x, float y, float srcX, float srcY, Color4 color)
   {
      this.position = new Vector2(x, y);
      this.source = new Vector2(srcX, srcY);
      this.color = color;
   }
}

struct ColoredVertex
{
   public const int Size = 6 * 4; // size of struct in bytes

   private readonly Vector2 position;
   private readonly Color4 color;

   public ColoredVertex(Vector2 position, Color4 color)
   {
      this.position = position;
      this.color = color;
   }
   public ColoredVertex(float x, float y, Color4 color)
   {
      this.position = new Vector2(x, y);
      this.color = color;
   }

   public override string ToString()
   {
      IntPtr block = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(50);
      System.Runtime.InteropServices.Marshal.StructureToPtr(this, block, false);
      byte[] bytes = new byte[64];
      System.Runtime.InteropServices.Marshal.Copy(block, bytes, 0, 64);
      System.Runtime.InteropServices.Marshal.FreeCoTaskMem(block);
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (int i = 0; i < bytes.Length; i += 4)
      {
         sb.Append(string.Format("({0})", BitConverter.ToSingle(bytes, i)));
      }
      return sb.ToString();
   }
}

/// <summary>
/// Collects vertices used in determining the locations of the corners of all the graphics being
/// drawn in a drawing operation.
/// </summary>
/// <typeparam name="TVertex"></typeparam>
sealed class VertexBuffer<TVertex> : IDisposable
where TVertex : struct // vertices must be structs so we can copy them to GPU memory easily
{
   private readonly int vertexSize;
   private TVertex[] vertices = new TVertex[4];

   private int count;

   private int handle = -1;

   public VertexBuffer(int vertexSize)
   {
      this.vertexSize = vertexSize;

      // generate the actual Vertex Buffer Object
      this.handle = GL.GenBuffer();
      Display.CheckError();
   }

   public void AddVertex(TVertex v)
   {
      // resize array if too small
      if (this.count == this.vertices.Length)
         Array.Resize(ref this.vertices, this.count * 2);
      // add vertex
      this.vertices[count] = v;
      this.count++;
   }

   public void Bind()
   {
      if (handle < 0)
         throw new ObjectDisposedException(this.GetType().Name);
      // make this the active array buffer
      GL.BindBuffer(BufferTarget.ArrayBuffer, this.handle);
      Display.CheckError();
   }

   public void BufferData()
   {
      // copy contained vertices to GPU memory
      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(this.vertexSize * this.count),
          this.vertices, BufferUsageHint.StreamDraw);
      Display.CheckError();
   }

   public void Draw(PrimitiveType primitive)
   {
      GL.DrawArrays(primitive, 0, this.count);
      Display.CheckError();
   }

   public void Clear()
   {
      count = 0;
   }

   #region IDisposable Support
   void Dispose(bool disposing)
   {
      if (handle >= 0)
      {
         int curHandle;
         GL.GetInteger(GetPName.ArrayBufferBinding, out curHandle);
         Display.CheckError();
         GL.DeleteBuffer(handle);
         Display.CheckError();
         if (curHandle == handle)
         {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Display.CheckError();
         }
         handle = -1;
      }
   }

   ~VertexBuffer()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
   }

   public void Dispose()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
   }
   #endregion
}

/// <summary>
/// Encapsulates an error occurring while compiling or linking an OpenGL shader.
/// </summary>
public class ShaderException : Exception
{
   public ShaderException(string message) : base(message) { }
}

/// <summary>
/// Encapsulates a single OpenGL shader, for example, a fragment shader or vertex shader.
/// </summary>
public sealed class Shader : IDisposable
{
   private int handle;

   public int Handle
   {
      get
      {
         if (this.handle < 0)
            throw new ObjectDisposedException(this.GetType().Name);
         return this.handle;
      }
   }

   private const string CodeVertexTextured = @"#version 130
      // a projection transformation to apply to the vertex' position
      uniform mat4 projectionMatrix;

      // attributes of our vertex
      in vec2 vPosition;
      in vec2 vSrc;
      in vec4 vColor;

      out vec2 vTex;  // must match name in fragment shader
      out vec4 fColor; // must match name in fragment shader

      void main()
      {
         // gl_Position is a special variable of OpenGL that must be set
         gl_Position = projectionMatrix * vec4(vPosition, -1.0, 1.0);
         vTex = vSrc;
         fColor = vColor;
      }";

   private static string CodeFragmentGeneral1 = @"#version 130
      in vec2 vTex; // must match name in vertex shader
      in vec4 fColor; // must match name in vertex shader

      out vec4 fragColor; // first out variable is automatically written to the screen

      uniform sampler2D tex;
      uniform sampler2D norm;

      #define MAX_LIGHTS " + LightSources.MAX_LIGHTS + @"
      #define MAX_WALLS " + LightSource.wallsPerLight + @"
                
      struct Light {
         vec3 position;
         vec4 color;
         vec3 falloff;
         vec3 aim;
         float aperture;
         float aperturesoftness;
         vec3 wall[MAX_WALLS * 2];
      };
                
      uniform Light lights[MAX_LIGHTS];

      void main()
      {
         vec4 DiffuseColor = texelFetch(tex, ivec2(vTex.x, vTex.y), 0);
         if (DiffuseColor.a == 0)
            discard;
         DiffuseColor *= fColor;";

   private const string CodeFragmentNormal2 = @"
         vec3 NormalMap = texelFetch(norm, ivec2(vTex.x, vTex.y), 0).rgb;";

   // We don't have vertically inverted normal maps, but if we did, this would account for it:
   // NormalMap.g = 1.0 - NormalMap.g;";

   private const string CodeFragmentGeneral3 = @"
         vec3 FinalColor = vec3(0,0,0);

         for (int i=0; i<MAX_LIGHTS; i++)
         {
            vec3 LightDir = vec3((lights[i].position.xy - gl_FragCoord.xy) / vec2(200.0, 200.0).xy, lights[i].position.z);
            float D = length(LightDir);";

   private const string CodeFragmentNormal4 = @"
            vec3 N = normalize(NormalMap * 2.0 - 1.0);
            vec3 L = normalize(LightDir);
            vec3 Diffuse = (lights[i].color.rgb * lights[i].color.a) * max(dot(N, L), 0.0);";

   private const string CodeFramentFlat4 = @"
            vec3 Diffuse = lights[i].color.rgb * lights[i].color.a;";


   private const string CodeFragmentGeneral5 = @"
            vec3 sd = normalize(vec3(gl_FragCoord.xy, lights[i].aim.z) - lights[i].position);
            vec3 a = normalize(lights[i].aim);
            float Attenuation = smoothstep(lights[i].aperture, lights[i].aperture + lights[i].aperturesoftness, dot(sd,a))
                  / (lights[i].falloff.x + (lights[i].falloff.y*D) + (lights[i].falloff.z*D*D) );
            vec3 Intensity = Diffuse * Attenuation;

            float shadow = 0;
            for (int w=0; w<MAX_WALLS; w++)
            {
               vec2 t2l = lights[i].position.xy - gl_FragCoord.xy;
               vec2 t2w0 = lights[i].wall[w*2].xy - gl_FragCoord.xy;
               vec2 t2w1 = lights[i].wall[w*2+1].xy - gl_FragCoord.xy;
               vec2 wall1 = lights[i].wall[w*2+1].xy - lights[i].wall[w*2].xy;
               vec2 w12l = lights[i].position.xy - lights[i].wall[w*2].xy;
               vec2 w12t = gl_FragCoord.xy - lights[i].wall[w*2].xy;
               float dp1 = dot(normalize(vec2(t2l.y, -t2l.x)), normalize(t2w0)); // >0 when ray from target to light intersects ray from wall vertex 0 to 1
               float dp2 = dot(normalize(vec2(-t2l.y, t2l.x)), normalize(t2w1)); // >0 when ray from target to light intersects ray from wall vertex 1 to 0
               float dp3 = 1-sign(abs(sign(dot(vec2(wall1.y, -wall1.x), w12l)) + sign(dot(vec2(wall1.y, -wall1.x), w12t)))); // 0 if light is on the same side of the wall, 1 otherwise
               float f1 = smoothstep(0, 2 - lights[i].wall[w*2].z * 2, dp1 * sign(dp2)) * smoothstep(0, 2 - lights[i].wall[w*2+1].z * 2, dp2 * sign(dp1));
               shadow = min(1, shadow + dp3 * f1); //smoothstep(-.002, 0, dp1 * dp2));
            }
            Intensity = (1 - shadow) * Intensity;
                  
            FinalColor += max(vec3(0,0,0), DiffuseColor.rgb * Intensity);
         }
         fragColor = vec4(FinalColor, DiffuseColor.a);
      }";

   private const string CodeVertexSolid = @"#version 130
      uniform mat4 projectionMatrix;
      in vec2 vPosition;
      in vec4 vColor;
      out vec4 fColor;
      void main()
      {
         gl_Position = projectionMatrix * vec4(vPosition, -1.0, 1.0);
         fColor = vColor;
      }";

   private const string CodeFragmentSolid = @"#version 130
      in vec4 fColor;
      out vec4 fragColor;            
      void main()
      {
         fragColor = fColor;
      }";

   private const string CodeFragmentNoLights = @"#version 130
      in vec2 vTex;  // must match name in vertex shader
      in vec4 fColor;
      out vec4 fragColor;            
      uniform sampler2D tex;
      void main()
      {
         vec4 DiffuseColor = texelFetch(tex, ivec2(vTex.x, vTex.y), 0);
         if (DiffuseColor.a == 0)
            discard;
         DiffuseColor *= fColor;
         fragColor = DiffuseColor;
      }";

   public enum ShaderCode
   {
      VertexShaderTextured,
      VertexShaderSolidColors,
      FragmentShaderWithNormals,
      FragmentShaderFlat,
      FragmentShaderSolid,
      FragmentShaderNoLights
   }

   public static Shader CreateShader(ShaderCode code)
   {
      switch (code)
      {
         case ShaderCode.VertexShaderTextured:
            return new Shader(ShaderType.VertexShader, CodeVertexTextured);
         case ShaderCode.VertexShaderSolidColors:
            return new Shader(ShaderType.VertexShader, CodeVertexSolid);
         case ShaderCode.FragmentShaderWithNormals:
            return new Shader(ShaderType.FragmentShader, CodeFragmentGeneral1 + CodeFragmentNormal2 +
               CodeFragmentGeneral3 + CodeFragmentNormal4 + CodeFragmentGeneral5);
         case ShaderCode.FragmentShaderFlat:
            return new Shader(ShaderType.FragmentShader, CodeFragmentGeneral1 + CodeFragmentGeneral3 +
               CodeFramentFlat4 + CodeFragmentGeneral5);
         case ShaderCode.FragmentShaderSolid:
            return new Shader(ShaderType.FragmentShader, CodeFragmentSolid);
         case ShaderCode.FragmentShaderNoLights:
            return new Shader(ShaderType.FragmentShader, CodeFragmentNoLights);
      }
      throw new ArgumentException("Unknown shader requested");
   }

   public Shader(ShaderType type, string code)
   {
      // create shader object
      this.handle = GL.CreateShader(type);

      // set source and compile shader
      GL.ShaderSource(this.handle, code);
      Display.CheckError();
      GL.CompileShader(this.handle);
      Display.CheckError();
      string info;
      GL.GetShaderInfoLog(this.handle, out info);
      if (!string.IsNullOrEmpty(info))
         throw new ShaderException(info);
   }

   #region IDisposable Support
   void Dispose(bool disposing)
   {
      if (handle >= 0)
      {
         GL.DeleteShader(handle);
         Display.CheckError();
         handle = -1;
      }
   }

   ~Shader()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
   }

   // This code added to correctly implement the disposable pattern.
   public void Dispose()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
   }
   #endregion
}

/// <summary>
/// Encapsulates an OpenGL shader program including, for example, both vertex and fragment shaders
/// </summary>
public sealed class ShaderProgram : IDisposable
{
   private int handle;

   private static ShaderProgram m_NormalMappedShader;
   private static ShaderProgram m_FlatShader;
   private static ShaderProgram m_SolidShader;
   private static ShaderProgram m_NoLightShader;

   public ShaderProgram(params Shader[] shaders)
   {
      // create program object
      this.handle = GL.CreateProgram();
      Display.CheckError();

      // assign all shaders
      foreach (var shader in shaders)
      {
         GL.AttachShader(this.handle, shader.Handle);
         Display.CheckError();
      }
      // link program (effectively compiles it)
      GL.LinkProgram(this.handle);
      Display.CheckError();
      string info;
      GL.GetProgramInfoLog(handle, out info);

      // detach shaders
      foreach (var shader in shaders)
      {
         GL.DetachShader(this.handle, shader.Handle);
         Display.CheckError();
      }

      if (!string.IsNullOrEmpty(info))
         throw new ShaderException(info);
   }

   private static void InitializeShaderPrograms()
   {
      Shader vs_textured = Shader.CreateShader(Shader.ShaderCode.VertexShaderTextured);
      Shader vs_solid = Shader.CreateShader(Shader.ShaderCode.VertexShaderSolidColors);
      Shader fs_normal = Shader.CreateShader(Shader.ShaderCode.FragmentShaderWithNormals);
      Shader fs_flat = Shader.CreateShader(Shader.ShaderCode.FragmentShaderFlat);
      Shader fs_solid = Shader.CreateShader(Shader.ShaderCode.FragmentShaderSolid);
      Shader fs_nolights = Shader.CreateShader(Shader.ShaderCode.FragmentShaderNoLights);

      m_NormalMappedShader = new ShaderProgram(vs_textured, fs_normal);
      m_FlatShader = new ShaderProgram(vs_textured, fs_flat);
      m_SolidShader = new ShaderProgram(vs_solid, fs_solid);
      m_NoLightShader = new ShaderProgram(vs_textured, fs_nolights);

      vs_textured.Dispose();
      vs_solid.Dispose();
      fs_normal.Dispose();
      fs_flat.Dispose();
      fs_solid.Dispose();
      fs_nolights.Dispose();
   }

   public static ShaderProgram NormalMappedShader
   {
      get
      {
         if (m_NormalMappedShader == null)
            InitializeShaderPrograms();
         return m_NormalMappedShader;
      }
   }

   public static ShaderProgram FlatShader
   {
      get
      {
         if (m_FlatShader == null)
            InitializeShaderPrograms();
         return m_FlatShader;
      }
   }

   public static ShaderProgram SolidShader
   {
      get
      {
         if (m_SolidShader == null)
            InitializeShaderPrograms();
         return m_SolidShader;
      }
   }

   public static ShaderProgram NoLightShader
   {
      get
      {
         if (m_NoLightShader == null)
            InitializeShaderPrograms();
         return m_NoLightShader;
      }
   }

   public static void DisposeShaderPrograms()
   {
      if (m_NormalMappedShader != null)
         m_NormalMappedShader.Dispose();
      if (m_FlatShader != null)
         m_FlatShader.Dispose();
      if (m_SolidShader != null)
         m_SolidShader.Dispose();
      if (m_NoLightShader != null)
         m_NoLightShader.Dispose();

      m_NormalMappedShader = null;
      m_FlatShader = null;
      m_SolidShader = null;
      m_NoLightShader = null;
   }

   public void Use(Matrix4 projectionMatrix)
   {
      if (handle < 0)
         throw new ObjectDisposedException(GetType().Name);
      // activate this program to be used
      GL.UseProgram(this.handle);
      GL.UniformMatrix4(GetUniformLocation("projectionMatrix"), false, ref projectionMatrix);
      Display.CheckError();
   }

   public int GetAttributeLocation(string name)
   {
      if (handle < 0)
         throw new ObjectDisposedException(GetType().Name);
      // get the location of a vertex attribute
      return GL.GetAttribLocation(this.handle, name);
   }

   public int GetUniformLocation(string name)
   {
      if (handle < 0)
         throw new ObjectDisposedException(GetType().Name);
      // get the location of a uniform variable
      int result = GL.GetUniformLocation(this.handle, name);
      if (result < 0)
         throw new ArgumentException("\"" + name + "\" not found.", name);
      return result;
   }

   #region IDisposable Support
   void Dispose(bool disposing)
   {
      if (handle >= 0)
      {
         GL.DeleteProgram(handle);
         Display.CheckError();
         handle = -1;
      }
   }

   ~ShaderProgram()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
   }

   // This code added to correctly implement the disposable pattern.
   public void Dispose()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
   }
   #endregion
}

/// <summary>
/// Links a collection of vertices to the current drawing operation and shader program.
/// </summary>
/// <typeparam name="TVertex">Determines the data type of each vertex</typeparam>
sealed class VertexArray<TVertex> : IDisposable
    where TVertex : struct
{
   private int handle;

   public VertexArray(VertexBuffer<TVertex> vertexBuffer, ShaderProgram program,
       params VertexAttribute[] attributes)
   {
      // create new vertex array object
      GL.GenVertexArrays(1, out handle);
      Display.CheckError();

      // bind the object so we can modify it
      Bind();

      // bind the vertex buffer object
      vertexBuffer.Bind();

      // set all attributes
      foreach (var attribute in attributes)
         attribute.Set(program);

      // unbind objects to reset state
      GL.BindVertexArray(0);
      Display.CheckError();
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      Display.CheckError();
   }

   public void Bind()
   {
      if (handle < 0)
         throw new ObjectDisposedException(GetType().Name);

      // bind for usage (modification or rendering)
      GL.BindVertexArray(handle);
      Display.CheckError();
   }

   #region IDisposable Support
   void Dispose(bool disposing)
   {
      if (handle >= 0)
      {
         int curVertexArray;
         GL.GetInteger(GetPName.VertexArrayBinding, out curVertexArray);
         Display.CheckError();
         if (curVertexArray == handle)
         {
            GL.BindVertexArray(0);
            Display.CheckError();
         }
         GL.DeleteVertexArrays(1, ref handle);
         Display.CheckError();
         handle = -1;
      }
   }

   ~VertexArray()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
   }

   // This code added to correctly implement the disposable pattern.
   public void Dispose()
   {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
   }
   #endregion
}

/// <summary>
/// Determines how attributes of vertices beign drawn are propagated to the shader program.
/// </summary>
sealed class VertexAttribute
{
   private readonly string name;
   private readonly int size;
   private readonly VertexAttribPointerType type;
   private readonly bool normalize;
   private readonly int stride;
   private readonly int offset;

   public VertexAttribute(string name, int size, VertexAttribPointerType type,
       int stride, int offset, bool normalize = false)
   {
      this.name = name;
      this.size = size;
      this.type = type;
      this.stride = stride;
      this.offset = offset;
      this.normalize = normalize;
   }

   public void Set(ShaderProgram program)
   {
      // get location of attribute from shader program
      int index = program.GetAttributeLocation(this.name);

      // enable and set attribute
      GL.EnableVertexAttribArray(index);
      Display.CheckError();
      if (type == VertexAttribPointerType.Int)
      {
         GL.VertexAttribIPointer(index, size, VertexAttribIntegerType.Int,
             stride, IntPtr.Add(IntPtr.Zero, offset));
         Display.CheckError();
      }
      else
      {
         GL.VertexAttribPointer(index, this.size, this.type,
             this.normalize, this.stride, this.offset);
         Display.CheckError();
      }
   }
}

/// <summary>
/// Aggregates light sources in a scene so that pixels can be affected by multiple light sources.
/// </summary>
class LightSources : IList<LightSource>
{
   /// <summary>
   /// Determines how many light sources can affect each drawing operation.
   /// </summary>
   /// <remarks>
   /// This must match MAX_LIGHTS in the fragment shader code.
   /// </remarks>
   public const int MAX_LIGHTS = 4;

   private List<LightSource> lights = new List<LightSource>(MAX_LIGHTS);
   private int[,] lightArrayLocations;
   ShaderProgram lastUsedProgram;

   /// <summary>
   /// Create a collection of light sources that can be applied to a ShaderProgram
   /// </summary>
   public LightSources()
   {
      for (int i = 0; i < MAX_LIGHTS; i++)
      {
         if (i == 0)
            lights.Add(new LightSource()
            {
               Position = new Vector3(0, 0, 1),
               Color = System.Drawing.Color.White,
               Falloff = new Vector3(1f, 0, 0),
               ApertureFocus = -10f,
               ApertureSoftness = 0f,
               Aim = new Vector3(1, 0, 0)
            });
         else
            lights.Add(new LightSource()
            {
               Position = new Vector3(),
               Color = System.Drawing.Color.Transparent,
               Falloff = new Vector3(.8f, .2f, 0f),
               ApertureFocus = -10f,
               ApertureSoftness = 0f,
               Aim = new Vector3(1, 0, 0)
            });
      }
   }

   /// <summary>
   /// Reset all light sources to initial default behavior
   /// </summary>
   public void Reset()
   {
      for (int i = 0; i < MAX_LIGHTS; i++)
      {
         if (i == 0)
         {
            lights[i].Position = new Vector3(0, 0, 1);
            lights[i].Color = System.Drawing.Color.White;
            lights[i].Falloff = new Vector3(1f, 0, 0);
         }
         else
         {
            lights[i].Position = new Vector3();
            lights[i].Color = System.Drawing.Color.Transparent;
            lights[i].Falloff = new Vector3(.8f, .2f, 0f);
         }
         lights[i].ApertureFocus = -10f;
         lights[i].ApertureSoftness = 0f;
         lights[i].Aim = new Vector3(1, 0, 0);
         for (int w = 0; w < LightSource.wallsPerLight * 2; w++)
         {
            lights[i][w] = new Vector3();
         }
      }
   }

   /// <summary>
   /// Prepare this collection of light sources to be used with the specified program.
   /// </summary>
   /// <param name="program">Shader program that uses light sources</param>
   /// <param name="lightArrayName">Variable name of the light source array in the shader program</param>
   public void UseProgram(ShaderProgram program, string lightArrayName)
   {
      if (program == lastUsedProgram)
         return;

      if ((lightArrayLocations == null) || (lightArrayLocations.GetUpperBound(0) < MAX_LIGHTS - 1)
         || (lightArrayLocations.GetUpperBound(1) < LightSource.locationCount - 1))
         lightArrayLocations = new int[MAX_LIGHTS, LightSource.locationCount];
      for (int i = 0; i < MAX_LIGHTS; i++)
      {
         lightArrayLocations[i, 0] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].position", i));
         lightArrayLocations[i, 1] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].color", i));
         lightArrayLocations[i, 2] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].falloff", i));
         lightArrayLocations[i, 3] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].aim", i));
         lightArrayLocations[i, 4] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].aperture", i));
         lightArrayLocations[i, 5] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].aperturesoftness", i));
         for (int w = 0; w < LightSource.wallsPerLight * 2; w++)
            lightArrayLocations[i, 6 + w] = program.GetUniformLocation(lightArrayName + string.Format("[{0}].wall[{1}]", i, w));
      }
      lastUsedProgram = program;
   }

   /// <summary>
   /// Access a single light source within this collection.
   /// </summary>
   /// <param name="index">Determines which 0-based light source is being accessed.</param>
   /// <returns>Light source object whose properties can be read or written.</returns>
   public LightSource this[int index]
   {
      get
      {
         return ((IList<LightSource>)lights)[index];
      }

      set
      {
         ((IList<LightSource>)lights)[index] = value;
      }
   }

   /// <summary>
   /// Returns the number of light sources in the collection, which is always MAX_LIGHTS.
   /// </summary>
   /// <remarks>
   /// The same number of light sources are always included in the scene's processing
   /// because, due to the way OpenGL uses parallelization to optimize the processing of
   /// pixels in the fragment shader, every pixel must undergo the same calculations.
   /// So in order to reduce the apparent number of light sources, a light source's
   /// properties are just set to have no effect on the scene instead of removing it
   /// entirely.
   /// </remarks>
   public int Count
   {
      get
      {
         return ((IList<LightSource>)lights).Count;
      }
   }

   /// <summary>
   /// Indicates whether this collection can be modified.
   /// </summary>
   public bool IsReadOnly
   {
      get
      {
         return ((IList<LightSource>)lights).IsReadOnly;
      }
   }

   /// <summary>
   /// Not supported. (Included only for IList interface.) <see cref="Count"/>
   /// </summary>
   public void Add(LightSource item)
   {
      throw new NotSupportedException("Number of light sources cannot be changed.");
   }

   /// <summary>
   /// Not supported. (Included only for IList interface.) <see cref="Count"/>
   /// </summary>
   public void Clear()
   {
      throw new NotSupportedException("Number of light sources cannot be changed.");
   }

   /// <summary>
   /// Determines whether the collection contains the specified light source object.
   /// </summary>
   /// <param name="item">Light source object for which to search.</param>
   /// <returns>True if the specified light source is in this collection.</returns>
   public bool Contains(LightSource item)
   {
      return ((IList<LightSource>)lights).Contains(item);
   }

   /// <summary>
   /// Copy the light sources in this collection to an array.
   /// </summary>
   /// <param name="array">Target array</param>
   /// <param name="arrayIndex">The 0-based starting index</param>
   public void CopyTo(LightSource[] array, int arrayIndex)
   {
      ((IList<LightSource>)lights).CopyTo(array, arrayIndex);
   }

   /// <summary>
   /// Allows this collection to be enumerated evaluating each light source in sequence.
   /// </summary>
   /// <returns>An object that can be used to enumerate all light sources in this collection.</returns>
   public IEnumerator<LightSource> GetEnumerator()
   {
      return ((IList<LightSource>)lights).GetEnumerator();
   }

   /// <summary>
   /// Determines where in the collection the specified light source occurs. <see cref="IList.IndexOf(object)"/>
   /// </summary>
   public int IndexOf(LightSource item)
   {
      return ((IList<LightSource>)lights).IndexOf(item);
   }

   /// <summary>
   /// Not supported. (Included only for IList interface.) <see cref="Count"/>
   /// </summary>
   public void Insert(int index, LightSource item)
   {
      throw new NotSupportedException("Number of light sources cannot be changed.");
   }

   /// <summary>
   /// Not supported. (Included only for IList interface.) <see cref="Count"/>
   /// </summary>
   public bool Remove(LightSource item)
   {
      throw new NotSupportedException("Number of light sources cannot be changed.");
   }

   /// <summary>
   /// Not supported. (Included only for IList interface.) <see cref="Count"/>
   /// </summary>
   public void RemoveAt(int index)
   {
      throw new NotSupportedException("Number of light sources cannot be changed.");
   }

   /// <summary>
   /// Allows this collection to be enumerated evaluating each light source in sequence.
   /// </summary>
   /// <returns>An object that can be used to enumerate all light sources in this collection.</returns>
   IEnumerator IEnumerable.GetEnumerator()
   {
      return ((IList<LightSource>)lights).GetEnumerator();
   }

   /// <summary>
   /// Applies the light sources in this collection to the current drawing operation.
   /// </summary>
   public void Set()
   {
      if (lightArrayLocations == null)
         throw new InvalidOperationException("LightSources.UseProgram must be called before LightSources.Set");
      for (int i = 0; i < lights.Count; i++)
      {
         GL.Uniform3(lightArrayLocations[i, 0], lights[i].Position);
         Display.CheckError();
         GL.Uniform4(lightArrayLocations[i, 1], lights[i].Color);
         Display.CheckError();
         GL.Uniform3(lightArrayLocations[i, 2], lights[i].Falloff);
         Display.CheckError();
         GL.Uniform3(lightArrayLocations[i, 3], lights[i].Aim);
         Display.CheckError();
         GL.Uniform1(lightArrayLocations[i, 4], lights[i].ApertureFocus);
         Display.CheckError();
         GL.Uniform1(lightArrayLocations[i, 5], lights[i].ApertureSoftness);
         Display.CheckError();
         for (int w = 0; w < LightSource.wallsPerLight * 2; w++)
         {
            GL.Uniform3(lightArrayLocations[i, 6 + w], lights[i][w]);
            Display.CheckError();
         }
      }
   }
}

/// <summary>
/// Defines the properties of a light source used by the SGDK2 display object
/// to determine brightness of drawn pixels at runtime.
/// </summary>
class LightSource
{
   /// <summary>
   /// Determines how many walls can obstruct each light source.
   /// </summary>
   public const int wallsPerLight = 20;
   /// <summary>
   /// Determines how many pointers OpenGL code needs to access
   /// all the properties of a light source.
   /// </summary>
   public const int locationCount = 6 + wallsPerLight * 2;

   private Vector3 position;
   private Color4 color;
   private Vector3 aim;
   private Vector3 falloff;
   private float apertureFocus;
   private float apertureSoftness;
   private Vector3[] wallVertices;

   public LightSource()
   {
      wallVertices = new Vector3[wallsPerLight * 2];
   }

   /// <summary>
   /// Pixel coordinate within the display where the light source resides.
   /// </summary>
   public Vector3 Position { get { return position; } set { position = value; } }

   /// <summary>
   /// Color of the light source with alpha representing brightness
   /// </summary>
   public System.Drawing.Color Color
   {
      get
      {
         return System.Drawing.Color.FromArgb(color.ToArgb());
      }

      set
      {
         color = new Color4(value.R, value.G, value.B, value.A);
      }
   }

   /// <summary>
   /// Coordinate relative to this light source's position at which the light is pointed
   /// </summary>
   public Vector3 Aim
   {
      get
      {
         return aim;
      }

      set
      {
         aim = value;
      }
   }

   /// <summary>
   /// Constant (x), linear (y) and quadratic (z) falloff coefficients for calculating attenuation
   /// </summary>
   public Vector3 Falloff
   {
      get
      {
         return falloff;
      }

      set
      {
         falloff = value;
      }
   }

   /// <summary>
   /// Determines whether the light is omni-directional, or emits light in limited directions.
   /// </summary>
   /// <remarks>
   /// A value of -1 emits light in all directions, 0 emits light over a 180-degree arc (making a linear
   /// divider between lit and unlight pixels at the light source), 0.7071 (cosine of 45
   /// degrees) results in a 90-degree arc. The actual apparent angle of illuminated pixels
   /// can vary based on the Aim because the arc is over a cone pointed by Aim.
   /// </remarks>
   public float ApertureFocus
   {
      get
      {
         return apertureFocus;
      }

      set
      {
         apertureFocus = value;
      }
   }

   /// <summary>
   /// For a directed and focused light source, determine how crisp the edges of its illumination are
   /// </summary>
   /// <remarks>
   /// A value of 0 results in every pixel along the edge of the cone being either entirely included
   /// or excluded from the light cone. A value of 0.1 results in smooth transition between cone sizes
   /// determined by ApertureFocus and ApertureFocus -0.1.
   /// </remarks>
   public float ApertureSoftness
   {
      get
      {
         return apertureSoftness;
      }

      set
      {
         apertureSoftness = value;
      }
   }

   /// <summary>
   /// Determines the endpoints of light barriers that block this light source's effect.
   /// </summary>
   /// <param name="index">Which barrier enpoint is being accessed (each barrier having 2 endpoints)</param>
   /// <returns>The location of an endpoint of a light barrier</returns>
   /// <remarks>
   /// Each barrier has 2 endpoints independent of other barriers, therefore, endpoints 0 and 1
   /// determine the configuration of the first barrier while endpoints 2 and 3 determine the
   /// second barrier. The Z coordinate of the barrier should be between 0 and 1 where 1 completely
   /// blocks light and lower values allow light to bleed in from that end of the barrier. 0 Causes
   /// the barrier to have no effect on the light source.
   /// </remarks>
   public Vector3 this[int index]
   {
      get { return wallVertices[index]; }
      set { wallVertices[index] = value; }
   }
}
