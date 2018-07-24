using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.ComponentModel;
using System.IO;

namespace BIT.XAF.CloudFileData.Dropbox
{
	//https://blogs.dropbox.com/developers/2015/06/introducing-a-preview-of-the-new-dropbox-net-sdk-for-api-v2/
	[DefaultProperty("FileName")]
	public class DropBoxFileData : BaseObject, IFileData, IEmptyCheckable
	{
		private string _DropboxPath;
		private string _Rev;
		private string fileName = "";
#if MediumTrust
		private int size;
		public int Size {
			get { return size; }
			set { SetPropertyValue("Size", ref size, value); }
		}
#else
		[Persistent]
		private int size;
        static string defaultFolder = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];
        public int Size
		{
			get { return size; }
		}
#endif
		public DropBoxFileData(Session session) : base(session) { }
		private static DropboxClient GetDroboxClient()
		{
            string AuthKey = System.Configuration.ConfigurationManager.AppSettings["DropboxAccessToken"];
            return new DropboxClient(AuthKey);
		}
		public virtual void LoadFromStream(string fileName, Stream stream)
		{

			// System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(Upload);
			//task.Start();
			//task.Wait();

			Guard.ArgumentNotNull(stream, "stream");
			FileName = fileName;
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			var metada = System.Threading.Tasks.Task.Run(() => Upload(defaultFolder, FileName, bytes));
			metada.Wait();
			this.size = (int)metada.Result.Size;
			this.DropboxPath = metada.Result.PathDisplay;
			this.Rev = metada.Result.Rev;
			//TODO add the sharing info



		}
		async System.Threading.Tasks.Task<FileMetadata> Upload(string folder, string file, byte[] bytes)
		{
			var dbx = GetDroboxClient();
			string FinalFileName = "/" + folder + "/" + file;
			//string FinalFileName =  file;
			FileMetadata Metadata = null;
			using (MemoryStream mem = new MemoryStream(bytes))
			{
                //TODO add metadata
                Metadata = await dbx.Files.UploadAsync(FinalFileName, WriteMode.Overwrite.Instance, false, DateTime.Now, true, null, mem);
			}
			return Metadata;

		}

		[Size(SizeAttribute.DefaultStringMappingFieldSize)]
		public string DropboxPath
		{
			get
			{
				return _DropboxPath;
			}
			set
			{
				SetPropertyValue("DropboxPath", ref _DropboxPath, value);
			}
		}
		[Size(SizeAttribute.DefaultStringMappingFieldSize)]
		public string Rev
		{
			get
			{
				return _Rev;
			}
			set
			{
				SetPropertyValue("Rev", ref _Rev, value);
			}
		}
		public virtual void SaveToStream(Stream stream)
		{

			var dbx = GetDroboxClient();
			var TaskResponse = System.Threading.Tasks.Task.Run(() => dbx.Files.DownloadAsync(string.Format("/{0}/{1}", defaultFolder, this.fileName)));
			TaskResponse.Wait();
			// byte[] TheFile = await response.GetContentAsByteArrayAsync();
			var TheFileTask = System.Threading.Tasks.Task.Run(() => TaskResponse.Result.GetContentAsByteArrayAsync());
			TheFileTask.Wait();
			var t = System.Threading.Tasks.Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, TheFileTask.Result, 0, Size, null);
			t.Wait();
			stream.Flush();


		}
		public void Clear()
		{
			//TODO delete
			//Content = null;


			var dbx = GetDroboxClient();
			var TaskResponse = System.Threading.Tasks.Task.Run(() => dbx.Files.DeleteAsync(string.Format("/{0}/{1}", defaultFolder, this.fileName)));
			TaskResponse.Wait();
			FileName = String.Empty;
		}
		public override string ToString()
		{
			return FileName;
		}
		[Size(260)]
		public string FileName
		{
			get { return fileName; }
			set { SetPropertyValue("FileName", ref fileName, value); }
		}
		
		#region IEmptyCheckable Members
		[NonPersistent, MemberDesignTimeVisibility(false)]
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(FileName); }
		}
		#endregion


	}
}