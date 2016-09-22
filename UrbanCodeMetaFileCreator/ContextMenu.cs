//using SharpShell.Attributes;
//using SharpShell.SharpContextMenu;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;

//namespace UrbanCodeMetaFileCreator
//{
//    [ComVisible(true)]
//    [COMServerAssociation(AssociationType.ClassOfExtension, ".txt")]
//    public class ContextMenu : SharpContextMenu
//    {
//        protected override bool CanShowMenu()
//        {
//            return true;
//        }

//        protected override ContextMenuStrip CreateMenu()
//        {
//            //  Create the menu strip.
//            var menu = new ContextMenuStrip();

//            //  Create a 'count lines' item.
//            var itemCountLines = new ToolStripMenuItem
//            {
//                Text = "Open Google"
//            };

//            //  When we click, we'll call the 'CountLines' function.
//            itemCountLines.Click += (sender, args) =>
//            {
//                System.Diagnostics.Process.Start("http://google.com");
//            };

//            //  Add the item to the context menu.
//            menu.Items.Add(itemCountLines);

//            //  Return the menu.
//            return menu;
//        }
//    }
//}
