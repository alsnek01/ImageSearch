using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace ImageSearch
{
	class CAppCapturTool
	{
		[DllImport( "User32", EntryPoint = "FindWindow" )]
		private static extern IntPtr FindWindow( string lpClassName, string lpWindowName );
		[DllImport( "user32" )]
		public static extern IntPtr FindWindowEx( IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2 );
		[DllImport( "user32.dll" )]
		internal static extern bool PrintWindow( IntPtr hWnd, IntPtr hdcBlt, int nFlags );

		public void SaveCapture( string findName )
		{
			var bmp = GetCapture( findName );
			bmp.Save( findName );
			bmp.Dispose();
		}
		public static Bitmap GetCapture( string findName )
		{
			IntPtr findwindow = getHandle( findName );
			if ( IntPtr.Zero == findwindow )
			{
				MessageBox.Show( $"[{findName}] 대상이 없습니다." );
				return null;
			}

			// graphics
			Graphics graphics = Graphics.FromHwnd( findwindow );
			Rectangle rt = Rectangle.Round( graphics.VisibleClipBounds );
			Bitmap bmp = new Bitmap( rt.Width, rt.Height );
			using ( Graphics g = Graphics.FromImage( bmp ) )
			{
				// capt
				IntPtr hdc = g.GetHdc();
				PrintWindow( findwindow, hdc, 0x2 );
				g.ReleaseHdc( hdc );
			}
			return bmp;
		}
		private static IntPtr getHandle( string window )
		{
			return FindWindow( null, window );
		}
		private static IntPtr getChildHandle( string window )
		{
			IntPtr hw1 = FindWindow( "LDPlayerMainFrame", window );
			IntPtr hw2 = FindWindowEx( hw1, IntPtr.Zero, null, "TheRender" );
			IntPtr hw3 = FindWindowEx( hw2, IntPtr.Zero, null, "sub" );
			return hw3;
		}
	}
	class CWindowCapturTool
	{
		public delegate void outCapture( string saveName, Bitmap bmp );

		public static void SaveCapture()
		{
			GetCaptur( ( saveName, bmp ) => 
			{
				bmp.Save( saveName );
				bmp.Dispose();
			} );
		}
		// 스크린 두개 이상
		public static void GetCaptur( outCapture fun )
		{
			var len = Screen.AllScreens.Length;
			for ( int i = 0; i < len; ++i )
			{
				var scr = Screen.AllScreens[i];

				var bitsPerPixel = Screen.PrimaryScreen.BitsPerPixel;
				var pixelFormat = PixelFormat.Format32bppArgb;
				if ( bitsPerPixel <= 16 )
				{
					pixelFormat = PixelFormat.Format16bppRgb565;
				}
				else if ( bitsPerPixel == 24 )
				{
					pixelFormat = PixelFormat.Format24bppRgb;
				}

				var rect = scr.Bounds;
				var bmp = new Bitmap( rect.Width, rect.Height, pixelFormat );
				using ( Graphics gr = Graphics.FromImage( bmp ) )
				{
					gr.CopyFromScreen( rect.Left, rect.Top, 0, 0, rect.Size );
				}

				fun?.Invoke( $"Captures{i}.png", bmp );
			}
		}
		// 스크린 하나
		public static Bitmap GetCaptur()
		{
			// pixel Format
			var bitsPerPixel = Screen.PrimaryScreen.BitsPerPixel;
			var pixelFormat = PixelFormat.Format32bppArgb;
			if ( bitsPerPixel <= 16 )
			{
				pixelFormat = PixelFormat.Format16bppRgb565;
			}
			else if ( bitsPerPixel == 24 )
			{
				pixelFormat = PixelFormat.Format24bppRgb;
			}

			// 화면 크기만큼의 Bitmap 생성
			var rect = Screen.PrimaryScreen.Bounds;
			var bmp = new Bitmap( rect.Width, rect.Height, pixelFormat );

			// Bitmap 이미지 변경을 위해 Graphics 객체 생성
			using ( Graphics gr = Graphics.FromImage( bmp ) )
			{
				// 화면 카피해서 Bitmap 메모리에 저장
				gr.CopyFromScreen( rect.Left, rect.Top, 0, 0, rect.Size );
			}

			return bmp;
		}

	}
}

