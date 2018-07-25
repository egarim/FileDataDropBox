<b>FileDataDropBox</b>

To make DropBoxFileData work you will need a dropbox api key and a authorization token
to get the api key you can go to this address https://dropbox.com/developers/apps
when you have the api key you will need to generate an authorization token you can use this utility for that


https://github.com/egarim/FileDataDropBox/blob/master/DropBoxAuthGenerator.zip

To get the authorization token use the following command

BitFwks.DropBoxAuthGenerator.exe YourApiKey

Once you have the authorization token you need to add some parameters on the app/web.config as shown below
<textarea>
<appSettings>
    <!-- ... -->
    <add key="DropBoxFileDataDefaultFolder" value="NameOfMyFolderOnDropBox"/>
    <add key="DropboxAccessToken" value="MyDropboxAccessToken"/>
    <!-- ... -->
  </appSettings>
</textarea>
  Now you can declare a DropBoxFileData as any other persistent property
  
        DropBoxFileData attachment;
        public DropBoxFileData Attachment
        {
            get => attachment;
            set => SetPropertyValue(nameof(Attachment), ref attachment, value);
        }
