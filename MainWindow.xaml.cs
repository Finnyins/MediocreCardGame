// Finn O'Brien
// Project 24 (Main window)
// 12/7/2021


using System;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using System.IO;

namespace Project24
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // These two doubles serve no purpose. They are remnants of a scrapped scaling method.
        double sizeX = 852;
        double sizeY = 480.8;

        WIN_Debug dbug = new WIN_Debug();

        bool simonwins = false;

        bool Debug = false;

        byte Gamemode = 3;

        List<string> CustomDeck1 = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild" };
        List<string> CustomDeck2 = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild" };

        List<MDL_Foil> Foils = StatsManager.LoadFoils();
        List<MDL_Stats> stats = StatsManager.Load();

        BitmapImage FoilTexture = new BitmapImage(new Uri("pack://application:,,,/Resources/FL_Default.png", UriKind.Absolute));

        BitmapImage DefaultFoil = new BitmapImage(new Uri("pack://application:,,,/Resources/FL_Default.png", UriKind.Absolute));

        BitmapImage CPUFoil = new BitmapImage(new Uri("pack://application:,,,/Resources/FL_CPU.png", UriKind.Absolute));

        byte SelectedFoil = 0;

        SCR_Deck PlrDeck = new SCR_Deck(false, false, 52, null);
        SCR_Deck CPUDeck = new SCR_Deck(false, false, 52, null);

        bool turn = false;
        bool defend = false;

        List<string> CPUHand = new List<string>();
        List<string> PlrHand = new List<string>();

        byte PlrPoints = 0;
        byte CPUPoints = 0;

        public MainWindow()
        {
            InitializeComponent();
            dbug.Close();
            LoadSettings();
            FoilTexture = new BitmapImage(new Uri($"pack://application:,,,{Foils[SelectedFoil].File}", UriKind.Absolute));
            RoutedCommand Debug = new RoutedCommand();
            Debug.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift));
            CommandBindings.Add(new CommandBinding(Debug, DebugToggle));
            RoutedCommand simon = new RoutedCommand();
            simon.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(simon, SimonWins));
            RoutedCommand Matchquit = new RoutedCommand();
            Matchquit.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(Matchquit, QuitMatch));
            RoutedCommand Fullscreen = new RoutedCommand();
            Fullscreen.InputGestures.Add(new KeyGesture(Key.F11));
            CommandBindings.Add(new CommandBinding(Fullscreen, FullscreenToggle));
            UpdateCards();
        }

        private void LoadSettings()
        {
            XDocument settings = new XDocument();

            if(!File.Exists("Settings.xml"))
            {
                settings = new XDocument
                    (
                    new XComment("User Settings Data"),
                    new XElement("Root",
                        new XElement("Fullscreen", CBX_Fullscreen.IsChecked),
                        new XElement("Animations", CBX_Animations.IsChecked),
                        new XElement("Foil_ID", SelectedFoil)
                        )
                    );
                settings.Save("Settings.xml");
            }
            else
            {
                settings = XDocument.Load("Settings.xml");
                CBX_Fullscreen.IsChecked = Convert.ToBoolean(settings.Root.Element("Fullscreen").Value);
                CBX_Animations.IsChecked = Convert.ToBoolean(settings.Root.Element("Animations").Value);
                SelectedFoil = Convert.ToByte(settings.Root.Element("Foil_ID").Value);
                CBX_Fullscreen_Click(null, null);
            }
        }

        private void SaveSettings()
        {
            XDocument settings = new XDocument();

            if (!File.Exists("Settings.xml"))
            {
                settings = new XDocument
                    (
                    new XComment("User Settings Data"),
                    new XElement("Root",
                        new XElement("Fullscreen", CBX_Fullscreen.IsChecked),
                        new XElement("Animations", CBX_Animations.IsChecked),
                        new XElement("Foil_ID", SelectedFoil)
                        )
                    );
                settings.Save("Settings.xml");
            }
            else
            {
                settings = XDocument.Load("Settings.xml");
                settings.Root.Element("Fullscreen").Value = Convert.ToString(CBX_Fullscreen.IsChecked);
                settings.Root.Element("Animations").Value = Convert.ToString(CBX_Animations.IsChecked);
                settings.Root.Element("Foil_ID").Value = Convert.ToString(SelectedFoil);
                settings.Save("Settings.xml");
            }
        }

        private async Task TakeFocus()
        {
            await Task.Delay(100);
            this.Focus();
        }

        private void FullscreenToggle(Object sender, ExecutedRoutedEventArgs a)
        {
            CBX_Fullscreen.IsChecked ^= true;
            CBX_Fullscreen_Click(null, null);
            SaveSettings();
        }

        private void QuitMatch(Object sender, ExecutedRoutedEventArgs a)
        {
            if (Gamemode != 3)
            {
                MessageBoxResult endmatch = MessageBox.Show("Quit match?\n\n\nYou will not be able to come back, and the results of this game will not be recorded.", "End Match", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (endmatch == MessageBoxResult.Yes)
                {
                    Gamemode = 3;
                    CNV_Play.IsEnabled = false;
                    CNV_Play.Visibility = Visibility.Hidden;
                    CNV_Menu.IsEnabled = true;
                    CNV_Menu.Visibility = Visibility.Visible;
                }
            }
            else if (Gamemode == 3)
            {
                BTN_Quit_Click(null, null);
            }
        }

        private void SimonWins(Object sender, ExecutedRoutedEventArgs a)
        {
            if (Debug)
            {
                if (simonwins)
                {
                    simonwins = false;
                    LBL_Header.Content = "The";
                    LBL_Title.Content = "Mediocre Card Game";
                    LBL_SubTitle.Content = "A Rudimentary Card Game of Sorts";
                    LBL_StatsTitle.Content = "Stats";
                }
                else
                {
                    simonwins = true;
                    LBL_Header.Content = "";
                    LBL_Title.Content = "Simon Wins";
                    LBL_SubTitle.Content = "Wow he's so good at this game";
                    LBL_StatsTitle.Content = "Simon Wins";
                }
                if (CNV_Stats.IsVisible)
                {
                    BTN_Stats_Click(null, null);
                }
            }
        }


        private void DebugToggle(Object sender, ExecutedRoutedEventArgs a)
        {
            if (Debug)
            {
                LBL_Debug.Visibility = Visibility.Hidden;
                BTN_Reset.IsEnabled = false;
                BTN_Reset.Visibility = Visibility.Hidden;
                Debug = false;
                simonwins = false;
                LBL_Header.Content = "The";
                LBL_Title.Content = "Mediocre Card Game";
                LBL_SubTitle.Content = "A Rudimentary Card Game of Sorts";
                LBL_StatsTitle.Content = "Stats";
                if (CNV_Stats.IsVisible)
                {
                    BTN_Stats_Click(null, null);
                }
                dbug.Close();
            }
            else
            {
                LBL_Debug.Visibility = Visibility.Visible;
                BTN_Reset.IsEnabled = true;
                BTN_Reset.Visibility = Visibility.Visible;
                Debug = true;
                dbug = new WIN_Debug();
                this.Focus();
                TakeFocus();
            }
        }

        private void ConsoleWriteLine(string inp)
        {
            dbug.DebugWriteLine(inp);
        }

        public void StartGame()
        {
            PlrPoints = 0;
            CPUPoints = 0;
            LBL_PlrPnt.Content = PlrPoints;
            LBL_CPUPnts.Content = CPUPoints;
            LBL_PlrDeck.Content = PlrDeck.cards.Count();
            LBL_CPUDeck.Content = CPUDeck.cards.Count();
            DisableCards();
            PlrHand.Clear();
            CPUHand.Clear();
            BTN_CardI.Content = "";
            BTN_CardII.Content = "";
            BTN_CardIII.Content = "";
            BTN_CardIV.Content = "";
            BTN_CardV.Content = "";
            LBL_Playcard.Content = "";
            LBL_Playcard.Visibility = Visibility.Hidden;
            BTN_Pass.IsEnabled = false;
            BTN_Pass.Visibility = Visibility.Hidden;
            GRD_Playcard.Background = new ImageBrush(DefaultFoil);
            DealCards();
            PlayerTurn();
        }
        private void DealCards()
        {
            for (int i = 0; i < 5; i++)
            {
                PlrHand.Add(PlrDeck.Draw());
                CPUHand.Add(CPUDeck.Draw());
            }
            BTN_CardI.Content = PlrHand[0];
            BTN_CardII.Content = PlrHand[1];
            BTN_CardIII.Content = PlrHand[2];
            BTN_CardIV.Content = PlrHand[3];
            BTN_CardV.Content = PlrHand[4];
            LBL_CPUDeck.Content = CPUDeck.cards.Count();
            LBL_PlrDeck.Content = PlrDeck.cards.Count();
            RCT_EcardI.Visibility = Visibility.Visible;
            RCT_EcardII.Visibility = Visibility.Visible;
            RCT_EcardIII.Visibility = Visibility.Visible;
            RCT_EcardIV.Visibility = Visibility.Visible;
            RCT_EcardV.Visibility = Visibility.Visible;
            BTN_CardI.Visibility = Visibility.Visible;
            BTN_CardII.Visibility = Visibility.Visible;
            BTN_CardIII.Visibility = Visibility.Visible;
            BTN_CardIV.Visibility = Visibility.Visible;
            BTN_CardV.Visibility = Visibility.Visible;
            if (Debug)
            {
                ReadCPUHand();
            }
        }
        
        private void EnableCards()
        {
            switch (PlrHand.Count)
            {
                case 5:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    BTN_CardIII.IsEnabled = true;
                    BTN_CardIV.IsEnabled = true;
                    BTN_CardV.IsEnabled = true;
                    break;

                case 4:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    BTN_CardIII.IsEnabled = true;
                    BTN_CardIV.IsEnabled = true;
                    break;

                case 3:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    BTN_CardIII.IsEnabled = true;
                    break;

                case 2:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    break;

                case 1:
                    BTN_CardI.IsEnabled = true;
                    break;
            }
            BTN_CardI.IsEnabled = true;
            BTN_CardII.IsEnabled = true;
            BTN_CardIII.IsEnabled = true;
            BTN_CardIV.IsEnabled = true;
            BTN_CardV.IsEnabled = true;
        }
        private void DisableCards()
        {
            BTN_CardI.IsEnabled = false;
            BTN_CardII.IsEnabled = false;
            BTN_CardIII.IsEnabled = false;
            BTN_CardIV.IsEnabled = false;
            BTN_CardV.IsEnabled = false;
        }
        private void PlayerTurn()
        {
            if (Gamemode == 3)
            {
                return;
            }
            if (Debug)
            {
                ConsoleWriteLine("Player Turn");
            }
            LBL_Who.Content = "Your";
            turn = true;
            EnableCards();
            if (PlrHand.Count == 0)
            {
                turn = false;
                PrepEndTurn(true, 0);
            }
        }

        async Task PlayerDefend()
        {
            if (Gamemode == 3)
            {
                return;
            }
            if (Debug)
            {
                ConsoleWriteLine("Player Defend");
            }
            LBL_Who.Content = "Your";
            LBL_Turn.Content = "Turn";
            defend = true;
            EnableCards();
            BTN_Pass.IsEnabled = true;
            BTN_Pass.Visibility = Visibility.Visible;
            if (PlrHand.Count == 0)
            {
                turn = false;
                if (LBL_Playcard.Content == "Wild")
                {
                    PrepEndTurn(false, 0);
                }
                else
                {
                    PrepEndTurn(false, Convert.ToByte(LBL_Playcard.Content));
                }
            }
        }

        private void CPUTurn()
        {
            if (Gamemode == 3)
            {
                return;
            }
            if (Debug)
            {
                ConsoleWriteLine("CPU Turn");
            }
            ReadCPUHand();
            byte highestcard = 0;
            byte cards = 0;
            bool hasonlywilds = true;
            foreach (string card in CPUHand)
            {
                cards++;
                if (card != "Wild")
                {
                    hasonlywilds = false;
                    if (Convert.ToByte(card) > highestcard)
                    {
                        highestcard = Convert.ToByte(card);
                    }
                }
            }
            if (cards == 0)
            {
                LBL_Playcard.Visibility = Visibility.Hidden;
            }
            if (hasonlywilds && cards != 0)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
                LBL_Playcard.Foreground = Brushes.Red;
                LBL_Playcard.Content = "Wild";
                CPUHand.Remove("Wild");
            }
            else if (cards != 0)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
                LBL_Playcard.Foreground = Brushes.Red;
                LBL_Playcard.Content = Convert.ToString(highestcard);
                CPUHand.Remove(Convert.ToString(highestcard));
            }
            else
            {
                LBL_Playcard.Visibility = Visibility.Hidden;
                LBL_Playcard.Content = 0;
            }
            GRD_Playcard.Background = new ImageBrush(CPUFoil);
            if (RCT_EcardV.IsVisible)
            {
                RCT_EcardV.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardIV.IsVisible)
            {
                RCT_EcardIV.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardIII.IsVisible)
            {
                RCT_EcardIII.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardII.IsVisible)
            {
                RCT_EcardII.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardI.IsVisible)
            {
                RCT_EcardI.Visibility = Visibility.Hidden;
            }
            if (cards != 0)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
            }
            if (LBL_Playcard.Content == "Wild")
            {
                PrepEndTurn(false, 0);
            }
            else if (cards != 0)
            {
                PlayerDefend();
            }
            else
            {
                PrepEndTurn(false, 0);
            }
        }

        async Task PrepCPUTurn()
        {
            await Task.Delay(2500);
            CPUTurn();
        }

        private void ReadCPUHand()
        {
            if (Debug)
            {
                string cards = "";
                foreach (string card in CPUHand)
                {
                    cards += $"{card}, ";
                }
                ConsoleWriteLine(cards);
            }
        }
        private void GRD_Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // This is nothing but the remnants of an old scaling setup
        }

        async Task CPUDraw()
        {
            if (CBX_Animations.IsChecked == true)
            {
                RCT_EcardV.RenderTransformOrigin = new Point(0.5, 0.5);
                RCT_EcardV.Visibility = Visibility.Visible;
                TranslateTransform trn = new TranslateTransform();
                DoubleAnimation topmove = new DoubleAnimation();
                topmove.From = 10;
                topmove.To = -4;
                topmove.Duration = TimeSpan.FromMilliseconds(150);
                DoubleAnimation leftmove = new DoubleAnimation();
                leftmove.From = -634;
                leftmove.To = -4;
                leftmove.Duration = TimeSpan.FromMilliseconds(200);
                Canvas.SetTop(RCT_EcardV, Canvas.GetTop(RCT_Edeck));
                Canvas.SetLeft(RCT_EcardV, Canvas.GetLeft(RCT_Edeck));
                trn.BeginAnimation(TranslateTransform.YProperty, topmove);
                trn.BeginAnimation(TranslateTransform.XProperty, leftmove);
                RCT_EcardV.RenderTransform = trn;
                Canvas.SetTop(RCT_EcardV, 10);
                Canvas.SetLeft(RCT_EcardV, 642);
                await Task.Delay(201);
                RCT_EcardV.RenderTransform = new ScaleTransform(1, 1);
            }
        }

        
        async Task DrawCard()
        {
            if (CBX_Animations.IsChecked == true)
            {
                BTN_CardV.RenderTransformOrigin = new Point(0.5, 0.5);
                BTN_CardV.Visibility = Visibility.Visible;
                TranslateTransform trn = new TranslateTransform();
                DoubleAnimation topmove = new DoubleAnimation();
                topmove.From = -61;
                topmove.To = 4;
                topmove.Duration = TimeSpan.FromMilliseconds(150);
                DoubleAnimation leftmove = new DoubleAnimation();
                leftmove.From = 106;
                leftmove.To = -4;
                leftmove.Duration = TimeSpan.FromMilliseconds(200);
                Canvas.SetTop(BTN_CardV, Canvas.GetTop(RCT_Deck));
                Canvas.SetLeft(BTN_CardV, Canvas.GetLeft(RCT_Deck));
                trn.BeginAnimation(TranslateTransform.YProperty, topmove);
                await Task.Delay(150);
                trn.BeginAnimation(TranslateTransform.XProperty, leftmove);
                BTN_CardV.RenderTransform = trn;
                Canvas.SetTop(BTN_CardV, 305);
                Canvas.SetLeft(BTN_CardV, 642);
                await Task.Delay(205);
                BTN_CardV.RenderTransform = new ScaleTransform(1, 1);
            }
        }

        async Task EndGame()
        {
            await Task.Delay(2500);
            Gamemode = 3;
            CNV_Victory.IsEnabled = false;
            CNV_Victory.Visibility = Visibility.Hidden;
            CNV_Play.IsEnabled = false;
            CNV_Play.Visibility = Visibility.Hidden;
            CNV_Menu.IsEnabled = true;
            CNV_Menu.Visibility = Visibility.Visible;
        }

        async Task EndTurn(bool plrscore, byte points)
        {
            if (Debug)
            {
                ConsoleWriteLine("End Turn");
            }
            if (Gamemode == 3)
            {
                return;
            }
            await Task.Delay(2500);
            LBL_Playcard.Visibility = Visibility.Hidden;
            GRD_Playcard.Background = new ImageBrush(DefaultFoil);
            BTN_Pass.IsEnabled = false;
            BTN_Pass.Visibility = Visibility.Hidden;
            if (plrscore)
            {
                PlrPoints += points;
                LBL_PlrPnt.Content = PlrPoints;
                LBL_Who.Content = "CPU's";
                LBL_Turn.Content = "Turn";
            }
            else
            {
                CPUPoints += points;
                LBL_CPUPnts.Content = CPUPoints;
                LBL_Who.Content = "Your";
                LBL_Turn.Content = "Turn";
            }
            string Plrdraw = null;
            if (PlrHand.Count < 5)
            {
                Plrdraw = PlrDeck.Draw();
            }
            if (Plrdraw != null)
            {
                PlrHand.Add(Plrdraw);
                BTN_CardV.Content = PlrHand[4];
                BTN_CardV.Visibility = Visibility.Visible;
                await DrawCard();
            }
            string CPUdraw = null;
            if (CPUHand.Count < 5)
            {
                CPUdraw = CPUDeck.Draw();
            }
            if (CPUdraw != null)
            {
                CPUHand.Add(CPUdraw);
                RCT_EcardV.Visibility = Visibility.Visible;
                RCT_EcardIV.Visibility = Visibility.Visible;
                RCT_EcardIII.Visibility = Visibility.Visible;
                await CPUDraw();
            }
            LBL_CPUDeck.Content = CPUDeck.cards.Count();
            LBL_PlrDeck.Content = PlrDeck.cards.Count();
            if (PlrHand.Count == 0 && CPUHand.Count == 0)
            {
                if (simonwins)
                {
                    LBL_Victor.Content = "Simon";
                    if (Gamemode != 4)
                    {
                        stats[Gamemode].Wins += 1;
                    }
                }
                else if (PlrPoints > CPUPoints)
                {
                    LBL_Victor.Content = "Player";
                    if (Gamemode != 4)
                    {
                        stats[Gamemode].Wins += 1;
                        if (Gamemode == 0 && CPUPoints == 0)
                        {

                        }
                    }

                }
                else if (PlrPoints < CPUPoints)
                {
                    LBL_Victor.Content = "CPU";
                    if (Gamemode != 4)
                    {
                        stats[Gamemode].Losses += 1;
                    }
                }
                else
                {
                    LBL_Victor.Content = "Draw. Nobody";
                    if (Gamemode != 4)
                    {
                        stats[Gamemode].Ties += 1;
                    }
                }
                CNV_Victory.Visibility = Visibility.Visible;
                if (Gamemode != 4)
                {
                    StatsManager.Update(stats[Gamemode]);
                }
                await EndGame();
                return;
            }
            ReadCPUHand();
            if (plrscore)
            {
                await PrepCPUTurn();
            }
            else
            {
                PlayerTurn();
            }
        }

        private async void PrepEndTurn(bool plrscore, byte points)
        {
            await EndTurn(plrscore, points);
        }



        private void CPUDefend()
        {
            if (Gamemode == 3)
            {
                return;
            }
            if (Debug)
            {
                ConsoleWriteLine("CPU Defend");
            }
            if (LBL_Playcard.Content == "Wild")
            {
                PrepEndTurn(true, 0);
            }
            else
            {
                sbyte remainder = 0;
                bool haswild = false;
                bool hasequal = false;
                bool hasgreater = false;
                bool hasnumbercard = false;
                bool playedcard = true;
                byte highestcard = 0;
                byte lowestcard = 10;
                byte lowestgreater = 10;
                byte highestless = 0;
                byte cards = 0;
                foreach (string card in CPUHand)
                {
                    cards++;
                    if (card != "Wild")
                    {
                        hasnumbercard = true;
                        if (Convert.ToByte(card) > highestcard)
                        {
                            highestcard = Convert.ToByte(card);
                        }
                        if (Convert.ToByte(card) < lowestcard)
                        {
                            lowestcard = Convert.ToByte(card);
                        }
                        if (Convert.ToByte(card) < Convert.ToByte(LBL_Playcard.Content) && Convert.ToByte(card) > highestless)
                        {
                            highestless = Convert.ToByte(card);
                        }
                    }
                    if (card == "Wild")
                    {
                        haswild = true;
                    }
                    else if (Convert.ToByte(card) == Convert.ToByte(LBL_Playcard.Content))
                    {
                        hasequal = true;
                    }
                    else if (Convert.ToByte(card) > Convert.ToByte(LBL_Playcard.Content))
                    {
                        hasgreater = true;
                        if (Convert.ToByte(card) < lowestgreater)
                        {
                            lowestgreater = Convert.ToByte(card);
                        }
                    }
                }
                if (cards == 0)
                {
                    remainder = Convert.ToSByte(LBL_Playcard.Content);
                }
                else if (haswild && Convert.ToByte(LBL_Playcard.Content) > 5 && CPUHand.Count != 0)
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = "Wild";
                    CPUHand.Remove("Wild");
                    remainder = 0;
                }
                else if (hasequal)
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    CPUHand.Remove(Convert.ToString(LBL_Playcard.Content));
                    remainder = 0;
                }
                else if (hasgreater && Convert.ToByte(LBL_Playcard.Content) <= 5 && lowestcard > Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) + 2))
                {
                    playedcard = false;
                    remainder = Convert.ToSByte(LBL_Playcard.Content);
                }
                else if (hasgreater && lowestcard > Convert.ToByte(LBL_Playcard.Content))
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestcard);
                    CPUHand.Remove(Convert.ToString(lowestcard));
                    remainder = 0;
                }
                else if (hasgreater && highestless < Convert.ToByte(Math.Max(0, Convert.ToByte(LBL_Playcard.Content) - 2)))
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                    remainder = 0;
                }
                else if (hasgreater && highestless >= Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) - 1) && lowestgreater > Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) + 2))
                {
                    remainder = Convert.ToSByte(Convert.ToByte(LBL_Playcard.Content) - highestless);
                    if (remainder < 0)
                    {
                        remainder = 0;
                    }
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(highestless);
                    CPUHand.Remove(Convert.ToString(highestless));
                }

                else if (hasgreater && highestless < Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) - 2))
                {
                    remainder = 0;
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                }
                else if (highestcard == 0 && CPUHand.Count > 0)
                {
                    remainder = 0;
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = "Wild";
                    CPUHand.Remove("Wild");
                }
                else if (hasgreater)
                {
                    remainder = 0;
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                }
                else if (hasnumbercard && highestcard != 0 && CPUHand.Count > 0)
                {
                    remainder = Convert.ToSByte(Convert.ToByte(LBL_Playcard.Content) - highestcard);
                    if (remainder < 0)
                    {
                        remainder = 0;
                    }
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(highestcard);
                    CPUHand.Remove(Convert.ToString(highestcard));
                }
                else
                {
                    playedcard = false;
                    remainder = Convert.ToSByte(LBL_Playcard.Content);
                }
                if (playedcard)
                {
                    if (CPUHand.Count != 0)
                    {
                        GRD_Playcard.Background = new ImageBrush(CPUFoil);
                    }
                    if (RCT_EcardV.IsVisible)
                    {
                        RCT_EcardV.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardIV.IsVisible)
                    {
                        RCT_EcardIV.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardIII.IsVisible)
                    {
                        RCT_EcardIII.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardII.IsVisible)
                    {
                        RCT_EcardII.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardI.IsVisible)
                    {
                        RCT_EcardI.Visibility = Visibility.Hidden;
                    }
                }
                PrepEndTurn(true, Convert.ToByte(remainder));
            }
        }

        async Task PrepCPUDefend()
        {
            await Task.Delay(2500);
            CPUDefend();
        }

        private async void CardClick(object sender, RoutedEventArgs e)
        {
            string value = "";
            string name = (sender as Button).Name.ToString();
            if (turn)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
                GRD_Playcard.Background = new ImageBrush(FoilTexture);
                switch (name)
                {
                    case "BTN_CardI":
                        value = PlrHand[0];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(0);
                        break;

                    case "BTN_CardII":
                        value = PlrHand[1];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(1);
                        break;

                    case "BTN_CardIII":
                        value = PlrHand[2];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(2);
                        break;

                    case "BTN_CardIV":
                        value = PlrHand[3];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(3);
                        break;

                    case "BTN_CardV":
                        value = PlrHand[4];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(4);
                        break;
                }
                if (BTN_CardV.IsVisible)
                {
                    BTN_CardV.IsEnabled = false;
                    BTN_CardV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                    BTN_CardIV.Content = PlrHand[3];
                }
                else if (BTN_CardIV.IsVisible)
                {
                    BTN_CardIV.IsEnabled = false;
                    BTN_CardIV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                }
                else if (BTN_CardIII.IsVisible)
                {
                    BTN_CardIII.IsEnabled = false;
                    BTN_CardIII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                }
                else if (BTN_CardII.IsVisible)
                {
                    BTN_CardII.IsEnabled = false;
                    BTN_CardII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                }
                else if (BTN_CardI.IsVisible)
                {
                    BTN_CardI.IsEnabled = false;
                    BTN_CardI.Visibility = Visibility.Hidden;
                }
                turn = false;
                LBL_Who.Content = "CPU's";
                LBL_Turn.Content = "Turn";
                DisableCards();
                await PrepCPUDefend();
            }
            else if (defend)
            {
                BTN_Pass.IsEnabled = false;
                BTN_Pass.Visibility = Visibility.Hidden;
                sbyte remainder = 0;
                LBL_Playcard.Visibility = Visibility.Visible;
                GRD_Playcard.Background = new ImageBrush(FoilTexture);
                byte played = Convert.ToByte(LBL_Playcard.Content);
                switch (name)
                {
                    case "BTN_CardI":
                        value = PlrHand[0];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(0);
                        break;

                    case "BTN_CardII":
                        value = PlrHand[1];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(1);
                        break;

                    case "BTN_CardIII":
                        value = PlrHand[2];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(2);
                        break;

                    case "BTN_CardIV":
                        value = PlrHand[3];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(3);
                        break;

                    case "BTN_CardV":
                        value = PlrHand[4];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(4);
                        break;
                }
                if (BTN_CardV.IsVisible)
                {
                    BTN_CardV.IsEnabled = false;
                    BTN_CardV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                    BTN_CardIV.Content = PlrHand[3];
                }
                else if (BTN_CardIV.IsVisible)
                {
                    BTN_CardIV.IsEnabled = false;
                    BTN_CardIV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                }
                else if (BTN_CardIII.IsVisible)
                {
                    BTN_CardIII.IsEnabled = false;
                    BTN_CardIII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                }
                else if (BTN_CardII.IsVisible)
                {
                    BTN_CardII.IsEnabled = false;
                    BTN_CardII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                }
                else if (BTN_CardI.IsVisible)
                {
                    BTN_CardI.IsEnabled = false;
                    BTN_CardI.Visibility = Visibility.Hidden;
                }
                if (value == "Wild")
                {
                    remainder = 0;
                }
                else
                {
                    remainder = Convert.ToSByte(played - Convert.ToByte(value));
                    if (remainder < 0)
                    {
                        remainder = 0;
                    }
                }
                defend = false;
                DisableCards();
                PrepEndTurn(false, Convert.ToByte(remainder));
            }
        }

        private void BTN_Pass_Click(object sender, RoutedEventArgs e)
        {
            if (defend)
            {
                BTN_Pass.IsEnabled = false;
                BTN_Pass.Visibility = Visibility.Hidden;
                DisableCards();
                if (PlrHand.Count != 0)
                {
                    if (LBL_Playcard.Content == "Wild")
                    {
                        defend = false;
                        EndTurn(false, 0);
                    }
                    else
                    {
                        defend = false;
                        EndTurn(false, Convert.ToByte(LBL_Playcard.Content));
                    }
                }
            }
        }

        private void BTN_PlayGame_Click(object sender, RoutedEventArgs e)
        {
            CNV_Menu.IsEnabled = false;
            CNV_Menu.Visibility = Visibility.Hidden;
            CNV_GameType.IsEnabled = true;
            CNV_GameType.Visibility = Visibility.Visible;
        }

        private void BTN_Classic_Click(object sender, RoutedEventArgs e)
        {
            Gamemode = 0;
            PlrDeck = new SCR_Deck(false, false, 52, CustomDeck1);
            CPUDeck = new SCR_Deck(false, false, 52, CustomDeck2);
            CNV_GameType.IsEnabled = false;
            CNV_GameType.Visibility = Visibility.Hidden;
            CNV_Play.Visibility = Visibility.Visible;
            CNV_Play.IsEnabled = true;
            StartGame();
        }

        private void BTN_Custom_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameType.IsEnabled = false;
            CNV_GameType.Visibility = Visibility.Hidden;
            CNV_GameSelect.IsEnabled = true;
            CNV_GameSelect.Visibility = Visibility.Visible;
        }

        private void BTN_Stats_Click(object sender, RoutedEventArgs e)
        {
            CNV_Menu.IsEnabled = false;
            CNV_Menu.Visibility = Visibility.Hidden;
            CNV_Stats.IsEnabled = true;
            CNV_Stats.Visibility = Visibility.Visible;
            if (Debug)
            {
                BTN_Reset.IsEnabled = true;
                BTN_Reset.Visibility = Visibility.Visible;
            }
            else
            {
                BTN_Reset.IsEnabled = false;
                BTN_Reset.Visibility = Visibility.Hidden;
            }
            if (Debug && simonwins)
            {
                LBL_ClassWin.Content = 1000;
                LBL_ClassLoss.Content = 0;
                LBL_ClassTies.Content = 0;
                LBL_ShareWin.Content = 1000;
                LBL_ShareLoss.Content = 0;
                LBL_ShareTie.Content = 0;
            }
            else
            {
                stats = StatsManager.Load();
                LBL_ClassWin.Content = stats[0].Wins;
                LBL_ClassLoss.Content = stats[0].Losses;
                LBL_ClassTies.Content = stats[0].Ties;
                LBL_ShareWin.Content = stats[1].Wins;
                LBL_ShareLoss.Content = stats[1].Losses;
                LBL_ShareTie.Content = stats[1].Ties;
            }
        }

        private void BTN_Quit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult quit = MessageBox.Show("Quit the game?\n\n\nHonestly I don't blame you", "Quit Game", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (quit == MessageBoxResult.Yes)
            {
                if (Debug)
                {
                    dbug.Close();
                }
                Close();
            }
        }

        private void BTN_BackCustom_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameSelect.IsEnabled = false;
            CNV_GameSelect.Visibility = Visibility.Hidden;
            CNV_GameType.IsEnabled = true;
            CNV_GameType.Visibility = Visibility.Visible;
        }

        private void BTN_Back_Click(object sender, RoutedEventArgs e)
        {
            CNV_Stats.IsEnabled = false;
            CNV_Stats.Visibility = Visibility.Hidden;
            CNV_Menu.IsEnabled = true;
            CNV_Menu.Visibility = Visibility.Visible;
        }

        private void BTN_Start_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameSelect.IsEnabled = false;
            CNV_GameSelect.Visibility = Visibility.Hidden;
            CNV_Play.IsEnabled = true;
            CNV_Play.Visibility = Visibility.Visible;
            bool custom = false;
            bool rdeck = false;
            byte decksize = Convert.ToByte(SLD_Cards.Value);
            if (CBX_Custom.IsChecked == true)
            {
                custom = true;
            }
            else if (CBX_Random.IsChecked == true && CBX_Custom.IsChecked == false)
            {
                rdeck = true;
            }
            if (custom == false && rdeck == false && CBX_Shared.IsChecked == false && decksize == 52)
            {
                Gamemode = 0;
            }
            else if (CBX_Custom.IsChecked == false && decksize > 51)
            {
                Gamemode = 1;
            }
            else
            {
                Gamemode = 4;
            }
            PlrDeck = new SCR_Deck(custom, rdeck, decksize, CustomDeck1);
            if (CBX_Shared.IsChecked == true)
            {
                CPUDeck = PlrDeck;
            }
            else
            {
                CPUDeck = new SCR_Deck(custom, rdeck, decksize, CustomDeck2);
            }
            ConsoleWriteLine($"Gamemode: {Gamemode}");
            StartGame();
        }

        private void BTN_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (Debug)
            {
                MessageBoxResult delete = MessageBox.Show("Delete data?", "Wipe Data", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (delete == MessageBoxResult.Yes)
                {
                    StatsManager.Reset();
                    BTN_Stats_Click(null, null);
                }
            }
        }

        private void FRM_Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Debug)
            {
                dbug.Close();
            }
        }

        private void BTN_BackToMain_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            CNV_Options.IsEnabled = false;
            CNV_Options.Visibility = Visibility.Hidden;
            CNV_Menu.IsEnabled = true;
            CNV_Menu.Visibility = Visibility.Visible;
        }

        private void BTN_Options_Click(object sender, RoutedEventArgs e)
        {
            CNV_Menu.IsEnabled = false;
            CNV_Menu.Visibility = Visibility.Hidden;
            CNV_Options.IsEnabled = true;
            CNV_Options.Visibility = Visibility.Visible;
        }

        private void CBX_Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (CBX_Fullscreen.IsChecked == true)
            {
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Maximized;
                this.Topmost = false;
            }
        }

        private void CBX_Custom_Click(object sender, RoutedEventArgs e)
        {
            if (CBX_Custom.IsChecked == true)
            {
                BTN_Customize.IsEnabled = true;
                SLD_Cards.IsEnabled = false;
                CBX_Random.IsEnabled = false;
                CBX_Random.IsChecked = false;
            }
            else
            {
                BTN_Customize.IsEnabled = false;
                SLD_Cards.IsEnabled = true;
                CBX_Random.IsEnabled = true;
            }
        }

        private void CardValue_Unfocused(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "" || (sender as TextBox).Text == null)
            {
                (sender as TextBox).Text = "0";
            }
            else
            {
                try
                {
                    Convert.ToByte((sender as TextBox).Text);
                }
                catch (System.FormatException)
                {
                    (sender as TextBox).Text = "0";
                }
                catch (System.InvalidCastException)
                {
                    (sender as TextBox).Text = "0";
                }
                catch (System.OverflowException)
                {
                    (sender as TextBox).Text = "255";
                }
            }
        }

        private void CardValue_Focused(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Clear();
        }

        private void CardValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).IsFocused == false)
            {
                try
                {
                    Convert.ToByte((sender as TextBox).Text);
                }
                catch (System.FormatException)
                {
                    (sender as TextBox).Text = "0";
                }
                catch (System.InvalidCastException)
                {
                    (sender as TextBox).Text = "0";
                }
                catch (System.OverflowException)
                {
                    (sender as TextBox).Text = "255";
                }
            }
        }

        private void BTN_ResetCards_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control txt in CNV_Deck1.Children)
            {
                if (txt is TextBox)
                {
                    (txt as TextBox).Text = "0";
                }
            }
            if (CBX_Shared.IsChecked == false)
            {
                foreach (Control txt in CNV_Deck2.Children)
                {
                    if (txt is TextBox)
                    {
                        (txt as TextBox).Text = "0";
                    }
                }
            }
        }

        private void BTN_ConfirmCards_Click(object sender, RoutedEventArgs e)
        {
            CustomDeck1.Clear();
            foreach (Control txt in CNV_Deck1.Children)
            {
                if (txt is TextBox)
                {
                    for (byte i = 0; i < Convert.ToByte((txt as TextBox).Text); i++)
                    {
                        CustomDeck1.Add(Convert.ToString(txt.Tag));
                    }
                }
            }
            ConsoleWriteLine(Convert.ToString(CustomDeck1.Count));
            if (CustomDeck1.Count < 10)
            {
                MessageBox.Show("ERROR: Insufficient deck size:\nDeck 1 has an invalid deck size. Each deck must have at least 10 cards total.", "Insufficient Deck Size", MessageBoxButton.OK, MessageBoxImage.Error);
                CustomDeck1.Clear();
                return;
            }
            if (CBX_Shared.IsChecked == false)
            {
                CustomDeck2.Clear();
                foreach (Control txt in CNV_Deck2.Children)
                {
                    if (txt is TextBox)
                    {
                        for (byte i = 0; i < Convert.ToByte((txt as TextBox).Text); i++)
                        {
                            CustomDeck2.Add(Convert.ToString(txt.Tag));
                        }
                    }
                }
                ConsoleWriteLine(Convert.ToString(CustomDeck2.Count));
                if (CustomDeck2.Count < 10)
                {
                    MessageBox.Show("ERROR: Insufficient deck size:\nDeck 2 has an invalid deck size. Each deck must have at least 10 cards total.", "Insufficient Deck Size", MessageBoxButton.OK, MessageBoxImage.Error);
                    CustomDeck2.Clear();
                    return;
                }
            }
            CNV_Customize.IsEnabled = false;
            CNV_Customize.Visibility = Visibility.Hidden;
            CNV_GameSelect.Visibility = Visibility.Visible;
            CNV_GameSelect.IsEnabled = true;
        }

        private void CBX_Shared_Click(object sender, RoutedEventArgs e)
        {
            if (CBX_Shared.IsChecked == true)
            {
                CNV_Deck2.IsEnabled = false;
            }
            else
            {
                CNV_Deck2.IsEnabled = true;
            }
        }

        private void CardHover(object sender, MouseEventArgs e)
        {
            (sender as Button).RenderTransformOrigin = new Point(0.5, 1);
            (sender as Button).RenderTransform = new ScaleTransform(1.15, 1.15, 0, 0);
            //(sender as Button).Width = 125;
            //(sender as Button).Height = 165;
        }

        private void NotCardHover(object sender, MouseEventArgs e)
        {
            (sender as Button).RenderTransformOrigin = new Point(0.5, 1);
            (sender as Button).RenderTransform = new ScaleTransform(1, 1, 0, 0);
            //(sender as Button).Width = 100;
            //(sender as Button).Height = 140;
        }

        private void BTN_Customize_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameSelect.IsEnabled = false;
            CNV_GameSelect.Visibility = Visibility.Hidden;
            CNV_Customize.Visibility = Visibility.Visible;
            CNV_Customize.IsEnabled = true;
            if (CBX_Shared.IsChecked == false)
            {
                CNV_Deck2.IsEnabled = true;
            }
        }

        private void BTN_GoBack_Click(object sender, RoutedEventArgs e)
        {
            CNV_Foils.IsEnabled = false;
            CNV_Foils.Visibility = Visibility.Hidden;
            CNV_Options.Visibility = Visibility.Visible;
            CNV_Options.IsEnabled = true;
        }

        private void UpdateCards()
        {
            BTN_CardI.Background = new ImageBrush(FoilTexture);
            BTN_CardII.Background = new ImageBrush(FoilTexture);
            BTN_CardIII.Background = new ImageBrush(FoilTexture);
            BTN_CardIV.Background = new ImageBrush(FoilTexture);
            BTN_CardV.Background = new ImageBrush(FoilTexture);
        }

        private void BTN_Foils_Click(object sender, RoutedEventArgs e)
        {
            List<MDL_Foil> Foils = StatsManager.LoadFoils();
            CNV_Options.IsEnabled = false;
            CNV_Options.Visibility = Visibility.Hidden;
            CNV_Foils.Visibility = Visibility.Visible;
            CNV_Foils.IsEnabled = true;
            BTN_FoilDisp.Background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,{Foils[SelectedFoil].File}", UriKind.Absolute)));
        }

        private void BTN_Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedFoil += 1;
                FoilTexture = new BitmapImage(new Uri($"pack://application:,,,{Foils[SelectedFoil].File}", UriKind.Absolute));
                BTN_FoilDisp.Background = new ImageBrush(FoilTexture);
                UpdateCards();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                SelectedFoil = 0;
                FoilTexture = new BitmapImage(new Uri($"pack://application:,,,{Foils[SelectedFoil].File}", UriKind.Absolute));
                BTN_FoilDisp.Background = new ImageBrush(FoilTexture);
                UpdateCards();
            }
        }

        private void BTN_Prev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedFoil -= 1;
                FoilTexture = new BitmapImage(new Uri($"pack://application:,,,{Foils[SelectedFoil].File}", UriKind.Absolute));
                BTN_FoilDisp.Background = new ImageBrush(FoilTexture);
                UpdateCards();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                SelectedFoil = Convert.ToByte(Foils.Count - 1);
                FoilTexture = new BitmapImage(new Uri($"pack://application:,,,{Foils[SelectedFoil].File}", UriKind.Absolute));
                BTN_FoilDisp.Background = new ImageBrush(FoilTexture);
                UpdateCards();
            }
        }

        private void BTN_Back2Menu_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameType.IsEnabled = false;
            CNV_GameType.Visibility = Visibility.Hidden;
            CNV_Menu.Visibility = Visibility.Visible;
            CNV_Menu.IsEnabled = true;
        }
    }
}
