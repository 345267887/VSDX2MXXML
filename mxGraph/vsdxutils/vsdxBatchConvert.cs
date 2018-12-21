using System;
using System.IO;

namespace mxGraph.vsdx.utils
{


    using mxVsdxCodec = mxGraph.io.mxVsdxCodec;
    using Utils = mxGraph.online.Utils;

    public class vsdxBatchConvert
    {

        /// <summary>
        /// Batch converts .vsdx files in the specified folder
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static void execute(java.io.File folder) throws java.io.IOException
        public static void execute(string filePath)
        {
            FileStream fs= File.OpenRead(filePath);
            byte[] fileData = new byte[fs.Length];
            fs.Read(fileData, 0, (int)fs.Length);

            if (fileData != null)
            {
                mxVsdxCodec vdxCodec = new mxVsdxCodec();
                string xml = null;

                try
                {
                    xml = vdxCodec.decodeVsdx(fileData, Utils.CHARSET_FOR_URL_ENCODING);
                }
                catch (Exception e)
                {
                    // TODO Auto-generated catch block
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }

                if (!string.ReferenceEquals(xml, null))
                {
                    Console.Write(xml);
                    //string path = StringUtils.substringBefore(file.Path, ".");

                    //using (PrintWriter @out = new PrintWriter(path + ".xml"))
                    //{
                    //    @out.println(xml);
                    //}
                }
            }

        }

        //private class FilenameFilterAnonymousInnerClass : FilenameFilter
        //{
        //	public FilenameFilterAnonymousInnerClass()
        //	{
        //	}

        //	public virtual bool accept(File dir, string filename)
        //	{
        //		return filename.EndsWith(".vsdx", StringComparison.Ordinal);
        //	}
        //}

        /// <summary>
        /// Shows a file dialog.
        /// </summary>
        //public static File selectFile(string title, string extension)
        //{
        //	JFileChooser chooser = new JFileChooser();
        //	// chooser.addChoosableFileFilter(new FileNameExtensionFilter(extension.toUpperCase() + " File", extension));
        //	chooser.FileSelectionMode = JFileChooser.DIRECTORIES_ONLY;
        //	chooser.DialogTitle = title;

        //	if (chooser.showOpenDialog(chooser) == JFileChooser.APPROVE_OPTION)
        //	{
        //		return chooser.SelectedFile;
        //	}

        //	return null;
        //}
    }

}