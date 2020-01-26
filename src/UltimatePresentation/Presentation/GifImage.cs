using System.Net;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UltimatePresentation.Presentation
{
	public class GifImageExceptionRoutedEventArgs : RoutedEventArgs
	{
		public Exception ErrorException;

		public GifImageExceptionRoutedEventArgs(RoutedEvent routedEvent, object obj)
		    : base(routedEvent, obj)
		{
		}
	}

	class WebReadState
	{
		public WebRequest webRequest;
		public MemoryStream memoryStream;
		public Stream readStream;
		public byte[] buffer;
	}


	public class GifImage : System.Windows.Controls.UserControl
	{
		private GifAnimation gifAnimation = null;
		private Image image = null;


		public GifImage()
		{
			this.Background = new SolidColorBrush(Colors.Transparent);
			this.Unloaded += GifImage_Unloaded;
		}

		private void GifImage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (gifAnimation != null)
			{
				gifAnimation.Dispose();
			}

			image = null;
			gifAnimation = null;
		}

		public static readonly DependencyProperty ForceGifAnimProperty = DependencyProperty.Register("ForceGifAnim", typeof(bool), typeof(GifImage), new FrameworkPropertyMetadata(false));
		public bool ForceGifAnim
		{
			get
			{
				return (bool)this.GetValue(ForceGifAnimProperty);
			}
			set
			{
				this.SetValue(ForceGifAnimProperty, value);
			}
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(GifImage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));
		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GifImage obj = (GifImage)d;
			obj.CreateFromSource();
		}

		public ImageSource Source
		{
			get
			{
				return (ImageSource)this.GetValue(SourceProperty);
			}
			set
			{
				this.SetValue(SourceProperty, value);
			}
		}

		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(GifImage), new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchChanged)));
		private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GifImage obj = (GifImage)d;
			Stretch s = (Stretch)e.NewValue;
			if (obj.gifAnimation != null)
			{
				obj.gifAnimation.Stretch = s;
			}
			else if (obj.image != null)
			{
				obj.image.Stretch = s;
			}
		}
		public Stretch Stretch
		{
			get
			{
				return (Stretch)this.GetValue(StretchProperty);
			}
			set
			{
				this.SetValue(StretchProperty, value);
			}
		}

		public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(GifImage), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchDirectionChanged)));
		private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GifImage obj = (GifImage)d;
			StretchDirection s = (StretchDirection)e.NewValue;
			if (obj.gifAnimation != null)
			{
				obj.gifAnimation.StretchDirection = s;
			}
			else if (obj.image != null)
			{
				obj.image.StretchDirection = s;
			}
		}
		public StretchDirection StretchDirection
		{
			get
			{
				return (StretchDirection)this.GetValue(StretchDirectionProperty);
			}
			set
			{
				this.SetValue(StretchDirectionProperty, value);
			}
		}

		public delegate void ExceptionRoutedEventHandler(object sender, GifImageExceptionRoutedEventArgs args);

		public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent("ImageFailed", RoutingStrategy.Bubble, typeof(ExceptionRoutedEventHandler), typeof(GifImage));

		public event ExceptionRoutedEventHandler ImageFailed
		{
			add
			{
				AddHandler(ImageFailedEvent, value);
			}
			remove
			{
				RemoveHandler(ImageFailedEvent, value);
			}
		}

		void image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
		{
			RaiseImageFailedEvent(e.ErrorException);
		}


		void RaiseImageFailedEvent(Exception exp)
		{
			GifImageExceptionRoutedEventArgs newArgs = new GifImageExceptionRoutedEventArgs(ImageFailedEvent, this);
			newArgs.ErrorException = exp;
			RaiseEvent(newArgs);
		}


		private void DeletePreviousImage()
		{
			if (image != null)
			{
				this.RemoveLogicalChild(image);
				image = null;
			}
			if (gifAnimation != null)
			{
				this.RemoveLogicalChild(gifAnimation);
				gifAnimation = null;
			}
			this.Content = null;
		}

		private void CreateNonGifAnimationImage()
		{
			DeletePreviousImage();
			var t = Source;
			image = null;
			if (t != null)
			{
				image = new Image();
				image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(image_ImageFailed);
				image.Source = t;
				image.Stretch = Stretch;
				image.StretchDirection = StretchDirection;
				this.AddChild(image);
			}
		}


		public void CreateGifAnimation(MemoryStream memoryStream)
		{
			DeletePreviousImage();
			gifAnimation = null;
			gifAnimation = new GifAnimation();
			gifAnimation.CreateGifAnimation(memoryStream);
			gifAnimation.Stretch = Stretch;
			gifAnimation.StretchDirection = StretchDirection;
			this.AddChild(gifAnimation);
		}

		private void CreateFromSource()
		{
			DeletePreviousImage();
			if (Source != null)
			{
				if (Source is BitmapFrame)
				{
					var metaData = Source.Metadata as BitmapMetadata;
					if (metaData != null)
					{
						if ("gif".Equals(metaData.Format, StringComparison.InvariantCultureIgnoreCase))
						{
							if (Source is BitmapFrame)
							{
								var uri = new Uri(new ImageSourceConverter().ConvertToString(Source));
								if (!CreateByUri(uri))
								{
									CreateNonGifAnimationImage();
									return;
								}
							}
							else
							{
								var bitmap = (BitmapImage)Source;
								if (bitmap.BaseUri != null)
								{
									if (!CreateByUri(bitmap.UriSource))
									{
										CreateNonGifAnimationImage();
									}
								}
								else if (bitmap.StreamSource != null)
								{
									ReadGifStreamSynch(bitmap.StreamSource);
								}
							}
							return;
						}
					}
				}

				CreateNonGifAnimationImage();
			}
		}

		private bool CreateByUri(Uri uri)
		{
			string leftPart = uri.GetLeftPart(UriPartial.Scheme);

			if (leftPart == "http://" || leftPart == "ftp://")
			{
				GetGifStreamFromHttp(uri);
			}
			else if (leftPart == "file://")
			{
				try
				{
					var memStream = new MemoryStream();
					using (var fs = File.OpenRead(uri.AbsolutePath))
					{
						fs.CopyTo(memStream);
					}
					memStream.Position = 0;
					CreateGifAnimation(memStream);
					return true;
				}
				catch
				{
					return false;
				}
			}
			else if (leftPart == "pack://")
			{
				GetGifStreamFromPack(uri);
			}
			else
			{
				return false;
			}
			return true;
		}

		private delegate void WebRequestFinishedDelegate(MemoryStream memoryStream);

		private void WebRequestFinished(MemoryStream memoryStream)
		{
			CreateGifAnimation(memoryStream);
		}

		private delegate void WebRequestErrorDelegate(Exception exp);

		private void WebRequestError(Exception exp)
		{
			RaiseImageFailedEvent(exp);
		}

		private void WebResponseCallback(IAsyncResult asyncResult)
		{
			WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
			WebResponse webResponse;
			try
			{
				webResponse = webReadState.webRequest.EndGetResponse(asyncResult);
				webReadState.readStream = webResponse.GetResponseStream();
				webReadState.buffer = new byte[100000];
				webReadState.readStream.BeginRead(webReadState.buffer, 0, webReadState.buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
			}
			catch (WebException exp)
			{
				this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
			}
		}

		private void WebReadCallback(IAsyncResult asyncResult)
		{
			WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
			int count = webReadState.readStream.EndRead(asyncResult);
			if (count > 0)
			{
				webReadState.memoryStream.Write(webReadState.buffer, 0, count);
				try
				{
					webReadState.readStream.BeginRead(webReadState.buffer, 0, webReadState.buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
				}
				catch (WebException exp)
				{
					this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
				}
			}
			else
			{
				this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestFinishedDelegate(WebRequestFinished), webReadState.memoryStream);
			}
		}

		private void GetGifStreamFromHttp(Uri uri)
		{
			try
			{
				WebReadState webReadState = new WebReadState();
				webReadState.memoryStream = new MemoryStream();
				webReadState.webRequest = WebRequest.Create(uri);
				webReadState.webRequest.Timeout = 10000;

				webReadState.webRequest.BeginGetResponse(new AsyncCallback(WebResponseCallback), webReadState);
			}
			catch (SecurityException)
			{
				CreateNonGifAnimationImage();
			}
		}


		private void ReadGifStreamSynch(Stream s)
		{
			byte[] gifData;
			MemoryStream memoryStream;
			using (s)
			{
				memoryStream = new MemoryStream((int)s.Length);
				BinaryReader br = new BinaryReader(s);
				gifData = br.ReadBytes((int)s.Length);
				memoryStream.Write(gifData, 0, (int)s.Length);
				memoryStream.Flush();
			}
			CreateGifAnimation(memoryStream);
		}

		private void GetGifStreamFromPack(Uri uri)
		{
			try
			{
				StreamResourceInfo streamInfo;

				if (!uri.IsAbsoluteUri)
				{
					streamInfo = Application.GetContentStream(uri);
					if (streamInfo == null)
					{
						streamInfo = Application.GetResourceStream(uri);
					}
				}
				else
				{
					if (uri.GetLeftPart(UriPartial.Authority).Contains("siteoforigin"))
					{
						streamInfo = Application.GetRemoteStream(uri);
					}
					else
					{
						streamInfo = Application.GetContentStream(uri);
						if (streamInfo == null)
						{
							streamInfo = Application.GetResourceStream(uri);
						}
					}
				}
				if (streamInfo == null)
				{
					throw new FileNotFoundException("Resource not found.", uri.ToString());
				}
				ReadGifStreamSynch(streamInfo.Stream);
			}
			catch (Exception exp)
			{
				RaiseImageFailedEvent(exp);
			}
		}
	}
}
