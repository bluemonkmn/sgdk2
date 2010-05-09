/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.Drawing;

/// <summary>
/// An instance of the Frame class represents one image from a frameset.
/// </summary>
public partial struct Frame
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
   /// Defines the transformed corners of this frame when drawn.
   /// </summary>
   public PointF[] Corners;
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
   /// <param name="corners">Corners of the output rectangle for drawing this frame in
   /// counter-clockwise order beginning with the top left corner.</param>
   /// <param name="srcRect">Provides this frame's <see cref="SourceRect"/>.</param>
   /// <param name="color">Provides this frame's <see cref="Color"/>.</param>
   public Frame(Display.TextureRef texture, short cell, PointF[] corners, Rectangle srcRect, int color)
   {
      GraphicSheetTexture = texture;
      CellIndex = cell;
      this.Corners = corners;
      SourceRect = srcRect;
      this.Color = color;
   }

   /// <summary>
   /// Creates a new frame definition based on a graphic sheet, cell index, transformation and source rectangle.
   /// </summary>
   /// <param name="texture">Refers to an object that provides a hardware copy of the graphic
   /// sheet on which this frame is based.</param>
   /// <param name="cell">Provides a value for this frame's <see cref="CellIndex"/> property.</param>
   /// <param name="corners">Corners of the output rectangle for drawing this frame in
   /// counter-clockwise order beginning with the top left corner.</param>
   /// <param name="srcRect">Provides this frame's <see cref="SourceRect"/>.</param>
   public Frame(Display.TextureRef texture, short cell, PointF[] corners, Rectangle srcRect) :
      this(texture, cell, corners, srcRect, -1)
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
      Corners = new PointF[] {
         new PointF(0, 0),
         new PointF(0, srcRect.Height),
         new PointF(srcRect.Width, srcRect.Height),
         new PointF(srcRect.Width, 0) };
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
