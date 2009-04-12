/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace SGDK2
{
   /// <summary>
   /// Summary description for CustTool.
   /// </summary>
   public class CustTool
   {
      private CustTool()
      {
      }
      
      public const int CustomToolCount = 10;

      public static String GetToolName(int ToolIndex)
      {
         switch (ToolIndex)
         {
            case 0:
               return "Star";
            case 1:
               return "Triangle";
            case 2:
               return "Diamond";
            case 3:
                return "Pentagon";
            case 4:
               return "Hexagon";
            case 5:
               return "Septagon";
            case 6:
               return "Octagon";
            case 7:
               return "Right Triangle";
            case 8:
               return "Droplet";
            case 9:
               return "Torus";
         }
         return "Unknown";
      }

      public static Bitmap GetToolImage(int ToolIndex)
      {
         Bitmap bmpResult = new Bitmap(15,15,PixelFormat.Format32bppArgb);
         using (Graphics g = Graphics.FromImage(bmpResult))
         {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            switch (ToolIndex)
            {
               case 0: // Star
               case 1: // Triangle
               case 2: // Diamond
               case 3: // Pentagon
               case 4: // Hexagon
               case 5: // Septagon
               case 6: // Octagon
                  DrawTool(ToolIndex, g, new PointF(7f, 7f), new PointF(7f, 0), Pens.Black, Brushes.Yellow,
                     ToolOptions.Outline | ToolOptions.Fill | ToolOptions.AntiAlias);
                  break;
               case 7: // Right Triangle
                  DrawTool(ToolIndex, g, new PointF(14f, 14f), new PointF(14f, 0), Pens.Black, Brushes.Yellow,
                     ToolOptions.Outline | ToolOptions.Fill | ToolOptions.AntiAlias);
                  break;
               case 8: // Droplet
                  DrawTool(ToolIndex, g, new PointF(7f, 14f), new PointF(7f, 0), Pens.Black, Brushes.Yellow,
                     ToolOptions.Outline | ToolOptions.Fill | ToolOptions.AntiAlias);
                  break;
               case 9:
                  DrawTool(ToolIndex, g, new PointF(0f, 0f), new PointF(14f, 14f), Pens.Black, Brushes.Yellow,
                     ToolOptions.Outline | ToolOptions.Fill | ToolOptions.AntiAlias);
                  break;
            }
         }
         return bmpResult;
      }
 
      public static void DrawTool(int ToolIndex, Graphics gTarget, PointF Start, PointF End, Pen CurrentPen, Brush CurrentBrush, ToolOptions Options)
      {
         double Angle = Math.Atan2(Start.Y -End.Y, End.X - Start.X);
         if (0 != (Options & ToolOptions.Lock))
            Angle = Math.Round(Angle * 8 / Math.PI) * Math.PI / 8;

         double Size = Math.Sqrt(Math.Pow(Start.Y - End.Y, 2) + Math.Pow(End.X - Start.X,2));
         PointF[] arpt;
         GraphicsPath pTemp;

         switch (ToolIndex)
         {
            case 0: // Star
               arpt = new PointF[10];
               for (int i = 0; i < 10; i++)
               {
                  arpt[i].X = (float)(Start.X + Math.Cos(Angle + i * Math.PI / 5.0) * Size * (1.6 - i % 2) / 1.6);
                  arpt[i].Y = (float)(Start.Y - Math.Sin(Angle + i * Math.PI / 5.0) * Size * (1.6 - i % 2) / 1.6);
               }
               pTemp = new GraphicsPath();
               pTemp.AddPolygon(arpt);
               DrawToolPath(gTarget, pTemp, CurrentPen, CurrentBrush, Options);
               pTemp.Dispose();
               break;
            case 1: // Triangle
            case 2: // Diamond
            case 3: // Pentagon
            case 4: // Hexagon
            case 5: // Septagon
            case 6: // Octagon
               arpt = new PointF[ToolIndex + 2];
               for (int i = 0; i < ToolIndex + 2; i++)
               {
                  arpt[i].X = (float)(Start.X + Math.Cos(Angle + i * 2 * Math.PI / (ToolIndex + 2)) * Size);
                  arpt[i].Y = (float)(Start.Y - Math.Sin(Angle + i * 2 * Math.PI / (ToolIndex + 2)) * Size);
               }
               pTemp = new GraphicsPath();
               pTemp.AddPolygon(arpt);
               DrawToolPath(gTarget, pTemp, CurrentPen, CurrentBrush, Options);
               pTemp.Dispose();
               break;
            case 7: // Right Triangle
               arpt = new PointF[3];
               arpt[0].X = (float)(Start.X + Math.Cos(Angle) * Size);
               arpt[0].Y = (float)(Start.Y - Math.Sin(Angle) * Size);
               arpt[1].X = (float)(Start.X + Math.Cos(Angle + Math.PI / 2) * Size);
               arpt[1].Y = (float)(Start.Y - Math.Sin(Angle + Math.PI / 2) * Size);
               arpt[2].X = (float)(Start.X);
               arpt[2].Y = (float)(Start.Y);
               pTemp = new GraphicsPath();
               pTemp.AddPolygon(arpt);
               DrawToolPath(gTarget, pTemp, CurrentPen, CurrentBrush, Options);
               pTemp.Dispose();
               break;
            case 8: // Droplet
               arpt = new PointF[7];
               arpt[0].X = (float)(Start.X + Math.Cos(Angle) * Size);
               arpt[0].Y = (float)(Start.Y - Math.Sin(Angle) * Size);
               arpt[1].X = (float)(Start.X + Math.Cos(Angle) * Size / 2);
               arpt[1].Y = (float)(Start.Y - Math.Sin(Angle) * Size / 2);
               arpt[2].X = (float)(Start.X + Math.Cos(Angle + Math.PI / 2.2) * Size/1.5);
               arpt[2].Y = (float)(Start.Y - Math.Sin(Angle + Math.PI / 2.2) * Size/1.5);
               arpt[3].X = (float)(Start.X);
               arpt[3].Y = (float)(Start.Y);
               arpt[4].X = (float)(Start.X + Math.Cos(Angle - Math.PI / 2.2) * Size/1.5);
               arpt[4].Y = (float)(Start.Y - Math.Sin(Angle - Math.PI / 2.2) * Size/1.5);
               arpt[5].X = (float)(Start.X + Math.Cos(Angle) * Size / 2);
               arpt[5].Y = (float)(Start.Y - Math.Sin(Angle) * Size / 2);
               arpt[6].X = (float)(Start.X + Math.Cos(Angle) * Size);
               arpt[6].Y = (float)(Start.Y - Math.Sin(Angle) * Size);
               pTemp = new GraphicsPath();
               pTemp.AddBeziers(arpt);
               DrawToolPath(gTarget, pTemp, CurrentPen, CurrentBrush, Options);
               pTemp.Dispose();
               break;
            case 9: // Torus
               pTemp = new GraphicsPath();
               pTemp.AddEllipse(Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y);
               pTemp.AddEllipse((Start.X * 3 + End.X) / 4f,
                                (Start.Y * 3 + End.Y) / 4f,
                                (End.X - Start.X) / 2f,
                                (End.Y - Start.Y) / 2f);
               if (((Options & (ToolOptions.GradientFill | ToolOptions.Fill)) ==
                  (ToolOptions.Fill | ToolOptions.GradientFill))
                  && (CurrentBrush is SolidBrush))
               {
                  if (((Math.Abs(Start.X - End.X) > 2) &&
                   (Math.Abs(Start.Y - End.Y) > 2)))
                  {
                     PathGradientBrush pgb = new PathGradientBrush(pTemp);
                     Blend blend = new Blend();
                     blend.Factors = new float[] { 0, 1, 0, 0 };
                     blend.Positions = new float[] { 0, 0.25F, .5F, 1 };
                     pgb.Blend = blend;
                     pgb.CenterColor = ((SolidBrush)CurrentBrush).Color;
                     pgb.SurroundColors = new Color[] { CurrentPen.Color };
                     gTarget.FillPath(pgb, pTemp);
                     pgb.Dispose();
                  }
                  if (0 != (Options & ToolOptions.Outline))
                     gTarget.DrawPath(CurrentPen, pTemp);
               }
               else
                  DrawToolPath(gTarget, pTemp, CurrentPen, CurrentBrush, Options);
               pTemp.Dispose();
               break;
         }
      }

      public static void DrawToolPath(Graphics gTarget, GraphicsPath DrawPath, Pen CurrentPen, Brush CurrentBrush, ToolOptions Options)
      {
         if (0 != (Options & ToolOptions.Fill))
         {
            if ((CurrentBrush is SolidBrush) && (0 != (Options & ToolOptions.GradientFill)))
            {
               if (IsOutlineLongEnoughToFill(DrawPath.PathPoints))
               {
                  PathGradientBrush pgb = new PathGradientBrush(DrawPath);
                  pgb.CenterColor = ((SolidBrush)CurrentBrush).Color;
                  pgb.SurroundColors = new Color[] {CurrentPen.Color};
                  gTarget.FillPath(pgb, DrawPath);
                  pgb.Dispose();
               }
            }
            else
               gTarget.FillPath(CurrentBrush, DrawPath);
         }
         if (0 != (Options & ToolOptions.Outline))
            gTarget.DrawPath(CurrentPen, DrawPath);
      }

      public static Boolean IsOutlineLongEnoughToFill(PointF[] pts)
      {
         float nLen = 0;
         for (int i=1; i< pts.Length; i++)
         {
            nLen += Math.Abs(pts[i].X - pts[i-1].X) + 
               Math.Abs(pts[i].Y - pts[i-1].Y);
            if (nLen > 6) return true;
         }
         return false;
      }
   }
}