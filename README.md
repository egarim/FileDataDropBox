To get the authorization token use the following command

BitFwks.DropBoxAuthGenerator.exe YourApiKey

Once you have the authorization token you need to add some parameters on the app/web.config as shown below

<appSettings>
    <!-- ... -->
    <add key="DropBoxFileDataDefaultFolder" value="NameOfMyFolderOnDropBox"/>
    <add key="DropboxAccessToken" value="MyDropboxAccessToken"/>
    <!-- ... -->
  </appSettings>

  Now you can declare a DropBoxFileData as any other persistent property
  
        DropBoxFileData attachment;
        public DropBoxFileData Attachment
        {
            get => attachment;
            set => SetPropertyValue(nameof(Attachment), ref attachment, value);
        }
