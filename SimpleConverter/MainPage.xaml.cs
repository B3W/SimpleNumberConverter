using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;

namespace SimpleConverter
{
    /// <summary>
    /// Main UI for converter app
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Flags to keep track of what conversion can happen
        private bool isAscii;
        private bool isBinary;
        private bool isDecimal;
        private bool isHex;
        // Flag to check if the flyout is open or not
        private bool flyoutIsOpen;

        public MainPage()
        {
            this.InitializeComponent();
            // Control window size
            ApplicationView.PreferredLaunchViewSize = new Size(450, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(450, 400));
            // Change Title Bar
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            // Set the BackgroundElement instead of the entire Titlebar grid
            Window.Current.SetTitleBar(BackgroundElement);
            // Event Handlers
            ConversionFlyoutBtn.Click += ConversionFlyoutBtn_Click;
            this.ConversionMenu.Opened += (sender, e) => flyoutIsOpen = true;
            this.ConversionMenu.Closed += (sender, e) => flyoutIsOpen = false;
            ExitBtn.Click += ExitBtn_Click;
            this.KeyUp += new KeyEventHandler(MainPage_KeyUp);
            Application.Current.Suspending += new SuspendingEventHandler(MainPage_OnSuspending);
            // Load previous state
            LoadState();
        } // MainPage

        /// <summary>
        /// Shows available conversion types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConversionFlyoutBtn_Click(object sender, RoutedEventArgs e)
        {
            SetStackPanel();
            this.ConversionMenu.ShowAt((FrameworkElement)sender);
        } // FlyBtn_Click
        
        /// <summary>
        /// Sets layout of the StackPanel to be shown in Flyout
        /// </summary>
        private void SetStackPanel()
        {
            string input = this.NumIn.Text.Trim();
            if (ValidInput(input))
            {
                this.FlyoutPanel.Children.Clear();
                if(isAscii)
                {
                    Button asciiBtn = new Button()
                    {
                        Content = "Letter to ASCII",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    asciiBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Letter to ASCII";
                        this.NumOut.Text = ((int)Convert.ToChar(input)).ToString();
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(asciiBtn);
                }
                if (isBinary)
                {
                    Button binDecimalBtn = new Button()
                    {
                        Content = "Binary to Decimal",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    binDecimalBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Binary to Decimal";
                        this.NumOut.Text = Convert.ToInt32(input, 2).ToString();
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(binDecimalBtn);
                    Button binHexBtn = new Button()
                    {
                        Content = "Binary to Hex",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    binHexBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Binary to Hex";
                        int decVal = Convert.ToInt32(input, 2);
                        this.NumOut.Text = string.Format("{0:x}", decVal);
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(binHexBtn);
                }
                if (isDecimal)
                {
                    Button decBinaryBtn = new Button()
                    {
                        Content = "Decimal to Binary",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    decBinaryBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Decimal to Binary";
                        int decValue = int.Parse(input);
                        this.NumOut.Text = Convert.ToString(decValue, 2);
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(decBinaryBtn);
                    Button decHexBtn = new Button()
                    {
                        Content = "Decimal to Hex",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    decHexBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Decimal to Hex";
                        int hexVal = int.Parse(this.NumIn.Text);
                        this.NumOut.Text = string.Format("{0:x}", hexVal);
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(decHexBtn);
                }
                if (isHex)
                {
                    Button hexBinaryBtn = new Button()
                    {
                        Content = "Hex to Binary",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    hexBinaryBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Hex to Binary";
                        long decVal = Convert.ToInt64(input, 16);
                        this.NumOut.Text = Convert.ToString(decVal, 2);
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(hexBinaryBtn);
                    Button hexDecimalBtn = new Button()
                    {
                        Content = "Hex to Decimal",
                        Background = new SolidColorBrush(Windows.UI.Colors.LightGray)
                    };
                    hexDecimalBtn.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.ConversionType.Text = "Hex to Decimal";
                        this.NumOut.Text = Convert.ToInt64(input, 16).ToString();
                        SaveState();
                    };
                    this.FlyoutPanel.Children.Add(hexDecimalBtn);
                }
            }
            else
            {
                this.FlyoutPanel.Children.Clear();
                this.FlyoutPanel.Children.Add(new TextBlock
                {
                    Text = "INVALID INPUT"
                });
            }
        } // SetStackPanel

        /// <summary>
        /// Determines if the entered values can be converted
        /// </summary>
        /// <param name="str">Input to be validated</param>
        /// <returns>Whether string is valid or not</returns>
        private bool ValidInput(string str)
        {
            if (str.Length != 0)
            {
                if (str.Length == 1)
                {
                    isAscii = true;
                } else
                {
                    isAscii = false;
                }
                isBinary = IsBinary(str);
                isDecimal = IsNumber(str);
                isHex = IsHex(str);
                return true;
            }
            return false;
        } // ValidInput

        /// <summary>
        /// Determines if the strin is binary numbers
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsBinary(string str)
        {
            Char[] chars = str.ToCharArray();
            bool validBinary;
            foreach (var c in chars)
            {
                validBinary = (c == '0' || c == '1');
                if (!validBinary)
                {
                    return false;
                }
            }
            return true;
        } // IsBinary

        /// <summary>
        /// Determines if string is a number
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsNumber(string str)
        {
            Char[] chars = str.ToCharArray();
            bool validNumber;
            foreach (var c in chars)
            {
                validNumber = (c >= '0' && c <= '9');
                if (!validNumber)
                {
                    return false;
                }
            }
            return true;
        } // IsNumber

        /// <summary>
        /// Determines if the string is only hexadecimal characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsHex(string str)
        {
            Char[] chars = str.ToCharArray();
            bool validHex;
            foreach (var c in chars)
            {
                validHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!validHex)
                    return false;
            }
            return true;
        } // IsHex

        /// <summary>
        /// Saves state of the application on suspending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            SaveState();
        }

        /// <summary>
        /// Closes down the application
        /// </summary>
        /// <param name="sender">Control sending request</param>
        /// <param name="e"></param>
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        } // ExitBtn_Click

        /// <summary>
        /// Handles escape and enter key presses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Windows.System.VirtualKey key = e.Key;
            if (key == Windows.System.VirtualKey.Escape)
            {
                if (!this.flyoutIsOpen)
                {
                    Application.Current.Exit();
                }
            }
            else if (key == Windows.System.VirtualKey.Enter)
            {
                ConversionFlyoutBtn_Click(this.ConversionFlyoutBtn, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Saves information on last converted number
        /// </summary>
        private async void SaveState()
        {
            string numConverted = this.NumIn.Text;
            string captionTxt = this.ConversionType.Text;
            string convertTxt = this.NumOut.Text;
            StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
            StorageFile userData = await appDataFolder.CreateFileAsync("SimpleConverterState.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(userData, numConverted + "\n" + captionTxt + "\n" + convertTxt);
        }

        /// <summary>
        /// Loads information from prevoius state
        /// </summary>
        private async void LoadState()
        {
            StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile userData = await appDataFolder.GetFileAsync("SimpleConverterState.txt");
                IList<string> fileLines = await FileIO.ReadLinesAsync(userData);
                this.NumIn.Text = fileLines[0];
                this.ConversionType.Text = fileLines[1];
                this.NumOut.Text = fileLines[2];
            }
            catch (Exception)
            {
                // Do nothing if no save file exists
            }
        }
    }
}
