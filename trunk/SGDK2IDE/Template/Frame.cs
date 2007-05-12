/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

/// <summary>
/// An instance of the Frame class represents one image from a frameset.
/// </summary>
public struct Frame
{
   /// <summary>
   /// References the un-transformed graphics on which this frame is based
   /// </summary>
   public Display.TextureRef GraphicSheetTexture;
   /// <summary>
   /// Refers to an index into the graphic sheet of the individual graphic on which this frame is based
   /// </summary>
   /// <remarks>This is informational only. The actual graphic is obtained using
   /// <see cref="SourceRect"/>.</remarks>
   public short CellIndex;
   /// <summary>
   /// Defines the transformation that is applied to the graphic cell to yield this frame
   /// </summary>
   public Matrix Transform;
   /// <summary>
   /// Specifies color channel modulations that are applied to this frame
   /// </summary>
   public int Color;
   /// <summary>
   /// Specifies the area on the graphic sheet which is used to form the graphic cell on which
   /// this frame is based.
   /// </summary>
   public Rectangle SourceRect;

   /// <summary>
   /// Creates a new frame definition based on a graphic sheet, cell index, transformation, source rectangle and color modulation.
   /// </summary>
   /// <param name="texture">Refers to an object that provides a hardware copy of the graphic
   /// sheet on which this frame is based.</param>
   /// <param name="cell">Provides a value for this frame's <see cref="CellIndex"/> property.</param>
   /// <param name="M11">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M12">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M21">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M22">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M41">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M42">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="srcRect">Provides this frame's <see cref="SourceRect"/>.</param>
   /// <param name="color">Provides this frame's <see cref="Color"/>.</param>
   public Frame(Display.TextureRef texture, short cell, float M11, float M12, float M21, float M22, float M41, float M42, Rectangle srcRect, int color)
   {
      GraphicSheetTexture = texture;
      CellIndex = cell;
      Transform = Matrix.Identity;
      Transform.M11 = M11;
      Transform.M12 = M12;
      Transform.M21 = M21;
      Transform.M22 = M22;
      Transform.M41 = M41;
      Transform.M42 = M42;
      Transform.M44 = 1;
      SourceRect = srcRect;
      this.Color = color;
   }

   /// <summary>
   /// Creates a new frame definition based on a graphic sheet, cell index, transformation and source rectangle.
   /// </summary>
   /// <param name="texture">Refers to an object that provides a hardware copy of the graphic
   /// sheet on which this frame is based.</param>
   /// <param name="cell">Provides a value for this frame's <see cref="CellIndex"/> property.</param>
   /// <param name="M11">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M12">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M21">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M22">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M41">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="M42">Provides a component of this frames <see cref="Transform"/> matrix.</param>
   /// <param name="srcRect">Provides this frame's <see cref="SourceRect"/>.</param>
   public Frame(Display.TextureRef texture, short cell, float M11, float M12, float M21, float M22, float M41, float M42, Rectangle srcRect) :
      this(texture, cell, M11, M12, M21, M22, M41, M42, srcRect, -1)
   {
   }

   /// <summary>
   /// Creates a new frame definition based on a graphic sheet, cell index, source rectangle and color modulation.
   /// </summary>
   /// <param name="texture">Refers to an object that provides a hardware copy of the graphic
   /// sheet on which this frame is based.</param>
   /// <param name="cell">Provides a value for this frame's <see cref="CellIndex"/> property.</param>
   /// <param name="srcRect">Provides this frame's <see cref="SourceRect"/>.</param>
   /// <param name="color">Provides this frame's <see cref="Color"/>.</param>
   /// <remarks>This constructor provides a shortcut for creating frames without transformations.</remarks>
   public Frame(Display.TextureRef texture, short cell, Rectangle srcRect, int color)
   {
      GraphicSheetTexture = texture;
      CellIndex = cell;
      Transform = Matrix.Identity;
      SourceRect = srcRect;
      this.Color = color;
   }

   /// <summary>
   /// Creates a new frame definition based on a graphic sheet, cell index and source rectangle.
   /// </summary>
   /// <param name="texture">Refers to an object that provides a hardware copy of the graphic
   /// sheet on which this frame is based.</param>
   /// <param name="cell">Provides a value for this frame's <see cref="CellIndex"/> property.</param>
   /// <param name="srcRect">Provides this frame's <see cref="SourceRect"/>.</param>
   /// <remarks>This constructor provides a shortcut for creating frames without transformations or color modulations.</remarks>
   public Frame(Display.TextureRef texture, short cell, Rectangle srcRect) :
      this(texture, cell, srcRect, -1)
   {
   }
}
