﻿using System;
using System.ComponentModel;

namespace SkiaSharp
{
	public enum SKPathConvexity
	{
		Unknown = 0,
		Convex = 1,
		Concave = 2,
	}

	public unsafe class SKPath : SKObject, ISKSkipObjectRegistration
	{
		internal SKPath (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

		public SKPath ()
			: this (SkiaApi.sk_path_new (), true)
		{
			if (Handle == IntPtr.Zero) {
				throw new InvalidOperationException ("Unable to create a new SKPath instance.");
			}
		}

		public SKPath (SKPath path)
			: this (SkiaApi.sk_path_clone (path.Handle), true)
		{
			if (Handle == IntPtr.Zero) {
				throw new InvalidOperationException ("Unable to copy the SKPath instance.");
			}
		}

		protected override void Dispose (bool disposing) =>
			base.Dispose (disposing);

		protected override void DisposeNative () =>
			SkiaApi.sk_path_delete (Handle);

		public SKPathFillType FillType {
			get => SkiaApi.sk_path_get_filltype (Handle);
			set => SkiaApi.sk_path_set_filltype (Handle, value);
		}

		public SKPathConvexity Convexity {
			get => IsConvex ? SKPathConvexity.Convex : SKPathConvexity.Concave;
			[Obsolete]
			set { }
		}

		public bool IsConvex => SkiaApi.sk_path_is_convex (Handle);

		public bool IsConcave => !IsConvex;

		public bool IsEmpty => VerbCount == 0;

		public bool IsOval => SkiaApi.sk_path_is_oval (Handle, null);

		public bool IsRoundRect => SkiaApi.sk_path_is_rrect (Handle, IntPtr.Zero);

		public bool IsLine => SkiaApi.sk_path_is_line (Handle, null);

		public bool IsRect => SkiaApi.sk_path_is_rect (Handle, null, null, null);

		public SKPathSegmentMask SegmentMasks => (SKPathSegmentMask)SkiaApi.sk_path_get_segment_masks (Handle);

		public int VerbCount => SkiaApi.sk_path_count_verbs (Handle);

		public int PointCount => SkiaApi.sk_path_count_points (Handle);

		public SKPoint this[int index] => GetPoint (index);

		public SKPoint[] Points => GetPoints (PointCount);

		public SKPoint LastPoint {
			get {
				SKPoint point;
				SkiaApi.sk_path_get_last_point (Handle, &point);
				return point;
			}
		}

		public SKRect Bounds {
			get {
				SKRect rect;
				SkiaApi.sk_path_get_bounds (Handle, &rect);
				return rect;
			}
		}

		public SKRect TightBounds {
			get {
				if (GetTightBounds (out var rect)) {
					return rect;
				} else {
					return SKRect.Empty;
				}
			}
		}

		public SKRect GetOvalBounds ()
		{
			SKRect bounds;
			if (SkiaApi.sk_path_is_oval (Handle, &bounds)) {
				return bounds;
			} else {
				return SKRect.Empty;
			}
		}

		public SKRoundRect GetRoundRect ()
		{
			var rrect = new SKRoundRect ();
			var result = SkiaApi.sk_path_is_rrect (Handle, rrect.Handle);
			if (result) {
				return rrect;
			} else {
				rrect.Dispose ();
				return null;
			}
		}

		public SKPoint[] GetLine ()
		{
			var temp = new SKPoint[2];
			fixed (SKPoint* t = temp) {
				var result = SkiaApi.sk_path_is_line (Handle, t);
				if (result) {
					return temp;
				} else {
					return null;
				}
			}
		}

		public SKRect GetRect () =>
			GetRect (out var isClosed, out var direction);

		public SKRect GetRect (out bool isClosed, out SKPathDirection direction)
		{
			byte c;
			fixed (SKPathDirection* d = &direction) {
				SKRect rect;
				var result = SkiaApi.sk_path_is_rect (Handle, &rect, &c, d);
				isClosed = c > 0;
				if (result) {
					return rect;
				} else {
					return SKRect.Empty;
				}
			}
		}

		public SKPoint GetPoint (int index)
		{
			if (index < 0 || index >= PointCount)
				throw new ArgumentOutOfRangeException (nameof (index));

			SKPoint point;
			SkiaApi.sk_path_get_point (Handle, index, &point);
			return point;
		}

		public SKPoint[] GetPoints (int max)
		{
			var points = new SKPoint[max];
			GetPoints (points, max);
			return points;
		}

		public int GetPoints (SKPoint[] points, int max)
		{
			fixed (SKPoint* p = points) {
				return SkiaApi.sk_path_get_points (Handle, p, max);
			}
		}

		public bool Contains (float x, float y) =>
			SkiaApi.sk_path_contains (Handle, x, y);

		public void Offset (SKPoint offset) =>
			Offset (offset.X, offset.Y);

		public void Offset (float dx, float dy) =>
			Transform (SKMatrix.CreateTranslation (dx, dy));

		public void MoveTo (SKPoint point) =>
			SkiaApi.sk_path_move_to (Handle, point.X, point.Y);

		public void MoveTo (float x, float y) =>
			SkiaApi.sk_path_move_to (Handle, x, y);

		public void RMoveTo (SKPoint point) =>
			SkiaApi.sk_path_rmove_to (Handle, point.X, point.Y);

		public void RMoveTo (float dx, float dy) =>
			SkiaApi.sk_path_rmove_to (Handle, dx, dy);

		public void LineTo (SKPoint point) =>
			SkiaApi.sk_path_line_to (Handle, point.X, point.Y);

		public void LineTo (float x, float y) =>
			SkiaApi.sk_path_line_to (Handle, x, y);

		public void RLineTo (SKPoint point) =>
			SkiaApi.sk_path_rline_to (Handle, point.X, point.Y);

		public void RLineTo (float dx, float dy) =>
			SkiaApi.sk_path_rline_to (Handle, dx, dy);

		public void QuadTo (SKPoint point0, SKPoint point1) =>
			SkiaApi.sk_path_quad_to (Handle, point0.X, point0.Y, point1.X, point1.Y);

		public void QuadTo (float x0, float y0, float x1, float y1) =>
			SkiaApi.sk_path_quad_to (Handle, x0, y0, x1, y1);

		public void RQuadTo (SKPoint point0, SKPoint point1) =>
			SkiaApi.sk_path_rquad_to (Handle, point0.X, point0.Y, point1.X, point1.Y);

		public void RQuadTo (float dx0, float dy0, float dx1, float dy1) =>
			SkiaApi.sk_path_rquad_to (Handle, dx0, dy0, dx1, dy1);

		public void ConicTo (SKPoint point0, SKPoint point1, float w) =>
			SkiaApi.sk_path_conic_to (Handle, point0.X, point0.Y, point1.X, point1.Y, w);

		public void ConicTo (float x0, float y0, float x1, float y1, float w) =>
			SkiaApi.sk_path_conic_to (Handle, x0, y0, x1, y1, w);

		public void RConicTo (SKPoint point0, SKPoint point1, float w) =>
			SkiaApi.sk_path_rconic_to (Handle, point0.X, point0.Y, point1.X, point1.Y, w);

		public void RConicTo (float dx0, float dy0, float dx1, float dy1, float w) =>
			SkiaApi.sk_path_rconic_to (Handle, dx0, dy0, dx1, dy1, w);

		public void CubicTo (SKPoint point0, SKPoint point1, SKPoint point2) =>
			SkiaApi.sk_path_cubic_to (Handle, point0.X, point0.Y, point1.X, point1.Y, point2.X, point2.Y);

		public void CubicTo (float x0, float y0, float x1, float y1, float x2, float y2) =>
			SkiaApi.sk_path_cubic_to (Handle, x0, y0, x1, y1, x2, y2);

		public void RCubicTo (SKPoint point0, SKPoint point1, SKPoint point2) =>
			SkiaApi.sk_path_rcubic_to (Handle, point0.X, point0.Y, point1.X, point1.Y, point2.X, point2.Y);

		public void RCubicTo (float dx0, float dy0, float dx1, float dy1, float dx2, float dy2) =>
			SkiaApi.sk_path_rcubic_to (Handle, dx0, dy0, dx1, dy1, dx2, dy2);

		public void ArcTo (SKPoint r, float xAxisRotate, SKPathArcSize largeArc, SKPathDirection sweep, SKPoint xy) =>
			SkiaApi.sk_path_arc_to (Handle, r.X, r.Y, xAxisRotate, largeArc, sweep, xy.X, xy.Y);

		public void ArcTo (float rx, float ry, float xAxisRotate, SKPathArcSize largeArc, SKPathDirection sweep, float x, float y) =>
			SkiaApi.sk_path_arc_to (Handle, rx, ry, xAxisRotate, largeArc, sweep, x, y);

		public void ArcTo (SKRect oval, float startAngle, float sweepAngle, bool forceMoveTo) =>
			SkiaApi.sk_path_arc_to_with_oval (Handle, &oval, startAngle, sweepAngle, forceMoveTo);

		public void ArcTo (SKPoint point1, SKPoint point2, float radius) =>
			SkiaApi.sk_path_arc_to_with_points (Handle, point1.X, point1.Y, point2.X, point2.Y, radius);

		public void ArcTo (float x1, float y1, float x2, float y2, float radius) =>
			SkiaApi.sk_path_arc_to_with_points (Handle, x1, y1, x2, y2, radius);

		public void RArcTo (SKPoint r, float xAxisRotate, SKPathArcSize largeArc, SKPathDirection sweep, SKPoint xy) =>
			SkiaApi.sk_path_rarc_to (Handle, r.X, r.Y, xAxisRotate, largeArc, sweep, xy.X, xy.Y);

		public void RArcTo (float rx, float ry, float xAxisRotate, SKPathArcSize largeArc, SKPathDirection sweep, float x, float y) =>
			SkiaApi.sk_path_rarc_to (Handle, rx, ry, xAxisRotate, largeArc, sweep, x, y);

		public void Close () =>
			SkiaApi.sk_path_close (Handle);

		public void Rewind () =>
			SkiaApi.sk_path_rewind (Handle);

		public void Reset () =>
			SkiaApi.sk_path_reset (Handle);

		public void AddRect (SKRect rect, SKPathDirection direction = SKPathDirection.Clockwise) =>
			SkiaApi.sk_path_add_rect (Handle, &rect, direction);

		public void AddRect (SKRect rect, SKPathDirection direction, uint startIndex)
		{
			if (startIndex > 3)
				throw new ArgumentOutOfRangeException (nameof (startIndex), "Starting index must be in the range of 0..3 (inclusive).");

			SkiaApi.sk_path_add_rect_start (Handle, &rect, direction, startIndex);
		}

		public void AddRoundRect (SKRoundRect rect, SKPathDirection direction = SKPathDirection.Clockwise)
		{
			if (rect == null)
				throw new ArgumentNullException (nameof (rect));
			SkiaApi.sk_path_add_rrect (Handle, rect.Handle, direction);
		}

		public void AddRoundRect (SKRoundRect rect, SKPathDirection direction, uint startIndex)
		{
			if (rect == null)
				throw new ArgumentNullException (nameof (rect));
			SkiaApi.sk_path_add_rrect_start (Handle, rect.Handle, direction, startIndex);
		}

		public void AddOval (SKRect rect, SKPathDirection direction = SKPathDirection.Clockwise) =>
			SkiaApi.sk_path_add_oval (Handle, &rect, direction);

		public void AddArc (SKRect oval, float startAngle, float sweepAngle) =>
			SkiaApi.sk_path_add_arc (Handle, &oval, startAngle, sweepAngle);

		public bool GetBounds (out SKRect rect)
		{
			var isEmpty = IsEmpty;
			if (isEmpty) {
				rect = SKRect.Empty;
			} else {
				fixed (SKRect* r = &rect) {
					SkiaApi.sk_path_get_bounds (Handle, r);
				}
			}
			return !isEmpty;
		}

		public SKRect ComputeTightBounds ()
		{
			SKRect rect;
			SkiaApi.sk_path_compute_tight_bounds (Handle, &rect);
			return rect;
		}

		public void Transform (SKMatrix matrix) =>
			SkiaApi.sk_path_transform (Handle, &matrix);

		public void Transform (SKMatrix matrix, SKPath destination)
		{
			if (destination == null)
				throw new ArgumentNullException (nameof (destination));

			SkiaApi.sk_path_transform_to_dest (Handle, &matrix, destination.Handle);
		}

		public void AddPath (SKPath other, float dx, float dy, SKPathAddMode mode = SKPathAddMode.Append)
		{
			if (other == null)
				throw new ArgumentNullException (nameof (other));

			SkiaApi.sk_path_add_path_offset (Handle, other.Handle, dx, dy, mode);
		}

		public void AddPath (SKPath other, ref SKMatrix matrix, SKPathAddMode mode = SKPathAddMode.Append)
		{
			if (other == null)
				throw new ArgumentNullException (nameof (other));

			fixed (SKMatrix* m = &matrix) {
				SkiaApi.sk_path_add_path_matrix (Handle, other.Handle, m, mode);
			}
		}

		public void AddPath (SKPath other, SKPathAddMode mode = SKPathAddMode.Append)
		{
			if (other == null)
				throw new ArgumentNullException (nameof (other));

			SkiaApi.sk_path_add_path (Handle, other.Handle, mode);
		}

		public void AddPathReverse (SKPath other)
		{
			if (other == null)
				throw new ArgumentNullException (nameof (other));

			SkiaApi.sk_path_add_path_reverse (Handle, other.Handle);
		}

		public void AddRoundRect (SKRect rect, float rx, float ry, SKPathDirection dir = SKPathDirection.Clockwise) =>
			SkiaApi.sk_path_add_rounded_rect (Handle, &rect, rx, ry, dir);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use AddRoundRect instead.")]
		public void AddRoundedRect (SKRect rect, float rx, float ry, SKPathDirection dir = SKPathDirection.Clockwise) =>
			AddRoundRect (rect, rx, ry, dir);

		public void AddCircle (float x, float y, float radius, SKPathDirection dir = SKPathDirection.Clockwise) =>
			SkiaApi.sk_path_add_circle (Handle, x, y, radius, dir);

		public void AddPoly (SKPoint[] points, bool close = true)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			fixed (SKPoint* p = points) {
				SkiaApi.sk_path_add_poly (Handle, p, points.Length, close);
			}
		}

		public Iterator CreateIterator (bool forceClose) =>
			new Iterator (this, forceClose);

		public RawIterator CreateRawIterator () =>
			new RawIterator (this);

		public bool Op (SKPath other, SKPathOp op, SKPath result)
		{
			if (other == null)
				throw new ArgumentNullException (nameof (other));
			if (result == null)
				throw new ArgumentNullException (nameof (result));

			return SkiaApi.sk_pathop_op (Handle, other.Handle, op, result.Handle);
		}

		public SKPath Op (SKPath other, SKPathOp op)
		{
			var result = new SKPath ();
			if (Op (other, op, result)) {
				return result;
			} else {
				result.Dispose ();
				return null;
			}
		}

		public bool Simplify (SKPath result)
		{
			if (result == null)
				throw new ArgumentNullException (nameof (result));

			return SkiaApi.sk_pathop_simplify (Handle, result.Handle);
		}

		public SKPath Simplify ()
		{
			var result = new SKPath ();
			if (Simplify (result)) {
				return result;
			} else {
				result.Dispose ();
				return null;
			}
		}

		public bool GetTightBounds (out SKRect result)
		{
			fixed (SKRect* r = &result) {
				return SkiaApi.sk_pathop_tight_bounds (Handle, r);
			}
		}

		public bool ToWinding (SKPath result)
		{
			if (result == null)
				throw new ArgumentNullException (nameof (result));

			return SkiaApi.sk_pathop_as_winding (Handle, result.Handle);
		}

		public SKPath ToWinding ()
		{
			var result = new SKPath ();
			if (ToWinding (result)) {
				return result;
			} else {
				result.Dispose ();
				return null;
			}
		}

		public string ToSvgPathData ()
		{
			using var str = new SKString ();
			SkiaApi.sk_path_to_svg_string (Handle, str.Handle);
			return (string)str;
		}

		public static SKPath ParseSvgPathData (string svgPath)
		{
			var path = new SKPath ();
			var success = SkiaApi.sk_path_parse_svg_string (path.Handle, svgPath);
			if (!success) {
				path.Dispose ();
				path = null;
			}
			return path;
		}

		public static SKPoint[] ConvertConicToQuads (SKPoint p0, SKPoint p1, SKPoint p2, float w, int pow2)
		{
			ConvertConicToQuads (p0, p1, p2, w, out var pts, pow2);
			return pts;
		}

		public static int ConvertConicToQuads (SKPoint p0, SKPoint p1, SKPoint p2, float w, out SKPoint[] pts, int pow2)
		{
			var quadCount = 1 << pow2;
			var ptCount = 2 * quadCount + 1;
			pts = new SKPoint[ptCount];
			return ConvertConicToQuads (p0, p1, p2, w, pts, pow2);
		}

		public static int ConvertConicToQuads (SKPoint p0, SKPoint p1, SKPoint p2, float w, SKPoint[] pts, int pow2)
		{
			if (pts == null)
				throw new ArgumentNullException (nameof (pts));
			fixed (SKPoint* ptsptr = pts) {
				return SkiaApi.sk_path_convert_conic_to_quads (&p0, &p1, &p2, w, ptsptr, pow2);
			}
		}

		//

		internal static SKPath GetObject (IntPtr handle, bool owns = true) =>
			handle == IntPtr.Zero ? null : new SKPath (handle, owns);

		//

		public class Iterator : SKObject, ISKSkipObjectRegistration
		{
			private readonly SKPath path;

			internal Iterator (SKPath path, bool forceClose)
				: base (SkiaApi.sk_path_create_iter (path.Handle, forceClose ? 1 : 0), true)
			{
				this.path = path;
			}

			protected override void Dispose (bool disposing) =>
				base.Dispose (disposing);

			protected override void DisposeNative () =>
				SkiaApi.sk_path_iter_destroy (Handle);

			[EditorBrowsable (EditorBrowsableState.Never)]
			[Obsolete ("Use Next(SKPoint[]) instead.")]
			public SKPathVerb Next (SKPoint[] points, bool doConsumeDegenerates, bool exact) =>
				Next (points);

			public SKPathVerb Next (SKPoint[] points) =>
				Next (new Span<SKPoint> (points));

			public SKPathVerb Next (Span<SKPoint> points)
			{
				if (points == null)
					throw new ArgumentNullException (nameof (points));
				if (points.Length != 4)
					throw new ArgumentException ("Must be an array of four elements.", nameof (points));

				fixed (SKPoint* p = points) {
					return SkiaApi.sk_path_iter_next (Handle, p);
				}
			}

			public float ConicWeight () =>
				SkiaApi.sk_path_iter_conic_weight (Handle);

			public bool IsCloseLine () =>
				SkiaApi.sk_path_iter_is_close_line (Handle) != 0;

			public bool IsCloseContour () =>
				SkiaApi.sk_path_iter_is_closed_contour (Handle) != 0;
		}

		public class RawIterator : SKObject, ISKSkipObjectRegistration
		{
			private readonly SKPath path;

			internal RawIterator (SKPath path)
				: base (SkiaApi.sk_path_create_rawiter (path.Handle), true)
			{
				this.path = path;
			}

			protected override void Dispose (bool disposing) =>
				base.Dispose (disposing);

			protected override void DisposeNative () =>
				SkiaApi.sk_path_rawiter_destroy (Handle);

			public SKPathVerb Next (SKPoint[] points) =>
				Next (new Span<SKPoint> (points));

			public SKPathVerb Next (Span<SKPoint> points)
			{
				if (points == null)
					throw new ArgumentNullException (nameof (points));
				if (points.Length != 4)
					throw new ArgumentException ("Must be an array of four elements.", nameof (points));
				fixed (SKPoint* p = points) {
					return SkiaApi.sk_path_rawiter_next (Handle, p);
				}
			}

			public float ConicWeight () =>
				SkiaApi.sk_path_rawiter_conic_weight (Handle);

			public SKPathVerb Peek () =>
				SkiaApi.sk_path_rawiter_peek (Handle);
		}

		public class OpBuilder : SKObject, ISKSkipObjectRegistration
		{
			public OpBuilder ()
				: base (SkiaApi.sk_opbuilder_new (), true)
			{
			}

			public void Add (SKPath path, SKPathOp op) =>
				SkiaApi.sk_opbuilder_add (Handle, path.Handle, op);

			public bool Resolve (SKPath result)
			{
				if (result == null)
					throw new ArgumentNullException (nameof (result));

				return SkiaApi.sk_opbuilder_resolve (Handle, result.Handle);
			}

			protected override void Dispose (bool disposing) =>
				base.Dispose (disposing);

			protected override void DisposeNative () =>
				SkiaApi.sk_opbuilder_destroy (Handle);
		}
	}
}
