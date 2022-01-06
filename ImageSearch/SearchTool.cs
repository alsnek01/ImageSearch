using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageSearch
{
	class CImageSearchTool
	{
		private static float m_accuracy = 0.8f;

		public static Bitmap OpenFindImage( Image view )
		{
			try
			{
				OpenFileDialog open = new OpenFileDialog();
				open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
				if ( open.ShowDialog() == DialogResult.OK )
				{
					return new Bitmap( open.FileName );
				}
			}
			catch ( Exception )
			{
				throw new ApplicationException( "Failed loading image" );
			}

			return null;
		}
		public static void Search( Bitmap captBmp, List<string> findItems )
		{
			if ( null == findItems || findItems.Count <= 0 )
				return;

			foreach ( var item in findItems )
			{
				var search = searchImg( captBmp, item );
				if ( !string.IsNullOrEmpty( search.findItem ) )
				{
					Console.WriteLine( $"찾음 [{search.findItem}]" );
				}
			}
		}
		private static (string findItem, int x, int y) searchImg( Bitmap captBitmap, string findName )
		{
			var findItem = "";
			var findBitmap = new Bitmap( findName );
			OpenCvSharp.Point minloc, maxloc;

			using ( Mat captMat = OpenCvSharp.Extensions.BitmapConverter.ToMat( captBitmap ) )
			using ( Mat findMat = OpenCvSharp.Extensions.BitmapConverter.ToMat( findBitmap ) )
			using ( Mat result = captMat.MatchTemplate( findMat, TemplateMatchModes.CCoeffNormed ) )
			{
				double minval, maxval = 0;

				Cv2.MinMaxLoc( result, out minval, out maxval, out minloc, out maxloc );
				Console.WriteLine( "찾은 이미지의 유사도 : " + maxval );

				if ( m_accuracy <= maxval )
				{
					findItem = findName;
				}
			}

			return (findItem, maxloc.X, maxloc.Y);
		}
		public static void SearchImageMark( string oriImg, string findImg )
		{
			using ( Mat mat = new Mat( oriImg ) )
			using ( Mat temp = new Mat( findImg ) )
			using ( Mat result = new Mat() )
			{
				// 이미지 템플릿 매치
				Cv2.MatchTemplate( mat, temp, result, TemplateMatchModes.CCoeffNormed );

				// 이미지의 최대/ 최소 위치 겟
				OpenCvSharp.Point minloc, maxloc;
				double minval, maxval;
				Cv2.MinMaxLoc( result, out minval, out maxval, out minloc, out maxloc );

				// 타겟 이미지랑 유사 정도 1에 가까울 수록 같음
				var threshold = 0.7;
				if ( maxval >= threshold )
				{
					// 서치된 부분을 빨간 테두리로
					OpenCvSharp.Rect rect = new OpenCvSharp.Rect( maxloc.X, maxloc.Y, temp.Width, temp.Height );
					Cv2.Rectangle( mat, rect, new OpenCvSharp.Scalar( 0, 0, 255 ), 2 );

					// 표시
					Cv2.ImShow( "template1_show", mat );
				}
				else
				{
					// 낫 매칭
					Console.WriteLine( "not find" );
				}
			}
		}
		public static void SearchImageMark( Bitmap oriImg, Bitmap findImg )
		{
			// cv 타입이 다를 수 있어 맞춰준다
			var grayOri = oriImg.ToGrayScale();
			var grayFind = oriImg.ToGrayScale();

			using ( Mat oritMat = OpenCvSharp.Extensions.BitmapConverter.ToMat( grayOri ) )
			using ( Mat findMat = OpenCvSharp.Extensions.BitmapConverter.ToMat( grayFind ) )
			using ( Mat result = new Mat() )
			{
				// 이미지 템플릿 매치
				Cv2.MatchTemplate( oritMat, findMat, result, TemplateMatchModes.CCoeffNormed );

				// 이미지의 최대/ 최소 위치 겟
				OpenCvSharp.Point minloc, maxloc;
				double minval, maxval;
				Cv2.MinMaxLoc( result, out minval, out maxval, out minloc, out maxloc );

				// 타겟 이미지랑 유사 정도 1에 가까울 수록 같음
				var threshold = 0.7;
				if ( maxval >= threshold )
				{
					// 서치된 부분을 빨간 테두리로
					OpenCvSharp.Rect rect = new OpenCvSharp.Rect( maxloc.X, maxloc.Y, findMat.Width, findMat.Height );
					Cv2.Rectangle( oritMat, rect, new OpenCvSharp.Scalar( 0, 0, 255 ), 2 );

					// 표시
					Cv2.ImShow( "template1_show", oritMat );
				}
				else
				{
					// 낫 매칭
					Console.WriteLine( "not find" );
				}
			}
		}
	}

	public static class CBMPHelper
	{
		public static Bitmap ToGrayScale( this Bitmap Bmp )
		{
			int rgb;
			Color c;

			for ( int y = 0; y < Bmp.Height; y++ )
			{
				for ( int x = 0; x < Bmp.Width; x++ )
				{
					c = Bmp.GetPixel( x, y );
					rgb = ( int )( ( c.R + c.G + c.B ) / 3 );
					Bmp.SetPixel( x, y, Color.FromArgb( rgb, rgb, rgb ) );
				}
			}
			return Bmp;
		}
	}
}
