using System.Text;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace test_note
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region ResizeWindows
        bool ResizeInProcess = false;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (ResizeInProcess)
            {
                Rectangle senderRect = sender as Rectangle;
                Window mainWindow = senderRect.Tag as Window;
                if (senderRect != null)
                {
                    double width = e.GetPosition(mainWindow).X;
                    double height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        width += 5;
                        if (width > 0)
                            mainWindow.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        width -= 5;
                        mainWindow.Left += width;
                        width = mainWindow.Width - width;
                        if (width > 0)
                        {
                            mainWindow.Width = width;
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        height += 5;
                        if (height > 0)
                            mainWindow.Height = height;
                    }
                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        height -= 5;
                        mainWindow.Top += height;
                        height = mainWindow.Height - height;
                        if (height > 0)
                        {
                            mainWindow.Height = height;
                        }
                    }
                }
            }
        }
        #endregion
        #region TitleButtons
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    App.Current.MainWindow.DragMove();
                }
            }
        }

        private void AdjustWindowSize()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
                MaximizeButton.Content = "";
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
                MaximizeButton.Content = "";
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }
        #endregion


        public SaveFileDialog sfd;
        public bool isSfd = false;
        public OpenFileDialog ofd;
        public bool isOfd = false;
        private string Webhook = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Text_Changed(object sender, RoutedEventArgs e)
        {
            if (!isSfd)
            {
                if (!isOfd)
                {
                    Filename.Content = "*Undefined.txt";
                }
                else
                {
                    Filename.Content = "*" + ofd.FileName;
                }
            }
            else
            {
                Filename.Content = "*" + sfd.FileName;
            }

            line.Content = "Line "+textWriter.GetLineIndexFromCharacterIndex(textWriter.CaretIndex);
            cols.Content = "Col " + (textWriter.CaretIndex - textWriter.GetCharacterIndexFromLineIndex(textWriter.GetLineIndexFromCharacterIndex(textWriter.CaretIndex)));
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"C:\";

            isOfd = true;

            if (ofd.ShowDialog()==true)
            {
                textWriter.Text = File.ReadAllText(ofd.FileName);
            }

            Filename.Content = ofd.FileName;
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (isSfd)
            {
                File.WriteAllText(sfd.FileName, textWriter.Text);
            }
            else
            {
                sfd = new SaveFileDialog();
                sfd.InitialDirectory = @"C:\";

                isSfd = true;

                if (sfd.ShowDialog() == true)
                {
                    File.WriteAllText(sfd.FileName, textWriter.Text);
                }
            }

            Filename.Content = sfd.FileName;
        }

        private void Webhook_Changed(object sender, RoutedEventArgs e)
        {
            Webhook = webhookInput.Text;
        }

        private void btnSendFile_Click(object sender, RoutedEventArgs e)
        {
            if (Webhook != "")
            {
                SendWebhook(textWriter.Text);
            }
            else
            {
                if (webhookInput.Visibility == Visibility.Hidden)
                {
                    btnSendFile.Content = "Send";
                    webhookInput.Visibility = Visibility.Visible;
                }
                else if (webhookInput.Visibility == Visibility.Visible)
                {
                    btnSendFile.Content = "Send?";
                    webhookInput.Visibility = Visibility.Hidden;

                    SendWebhook(textWriter.Text);
                }
            }
        }

        private void SendWebhook(string msg)
        {
            string fmsg = msg.Replace("\r\n", "\\n");
            string payload = "{\"content\": \"" + fmsg + "\"}";
            Debug.WriteLine(payload);

            WebClient client = new WebClient();

            Debug.WriteLine(Webhook);

            client.Headers.Add("Content-Type", "application/json");
            client.UploadData(Webhook, Encoding.UTF8.GetBytes(payload));
        }
    }
}
