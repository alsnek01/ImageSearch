using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageSearch
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click( object sender, EventArgs e )
		{
		}

		/*======================================================================================================
			sample : 이미지파일 두개로 찾기
		======================================================================================================*/
		/*private void button1_Click( object sender, EventArgs e )
		{
			var oriBmp = new Bitmap( @"../res\cal.png" );
			var findBmp = new Bitmap( @"../res\num5.png" );
			CImageSearchTool.SearchImageMark( oriBmp, findBmp );
		}*/

		/*======================================================================================================
			sample : window 이미지 찾기 
		======================================================================================================*/
		/*private void button1_Click( object sender, EventArgs e )
		{
			var captureBmp = CWindowCapturTool.GetCaptur();
			var findBmp = new Bitmap( @"../res\num5.png" );
			CImageSearchTool.SearchImageMark( captureBmp, findBmp );
		}*/

		/*======================================================================================================
			sample : window 캡쳐
		======================================================================================================*/
		/*private void button1_Click( object sender, EventArgs e )
		{
			CWindowCapturTool.SaveCapture();
		}*/
	}
}
