using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.CognitiveServices.Speech;
using System.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Speech.Recognition;
using System.Threading;
using Grammar = System.Speech.Recognition.Grammar;
using System.Windows.Controls.Primitives;
using MyNotes.ViewModel;
using MyNotes.ViewModel.Helpers;
using System.IO;

namespace MyNotes.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        NotesVM viewModel;
        SpeechRecognitionEngine recognizer;

        //SpeechRecognitionEngine recognizer;
        public NotesWindow()
        {
            InitializeComponent();
            viewModel = Resources["vm"] as NotesVM;
            viewModel.SelectedNoteChanged += ViewModel_SelectedNoteChanged;

            #region
            //var currentCulture = (from r in SpeechRecognitionEngine.InstalledRecognizers()
            //                      where r.Culture.Equals(Thread.CurrentThread.CurrentCulture)
            //                      select r).FirstOrDefault();
            //recognizer = new SpeechRecognitionEngine(currentCulture);


            RecognizerInfo ri = null;
            foreach (var i in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (i.Culture.TwoLetterISOLanguageName.Equals("en"))
                {
                    ri = i;
                }
            }

            recognizer = new SpeechRecognitionEngine(ri);

            GrammarBuilder builder = new GrammarBuilder();
            builder.AppendDictation();
            Grammar grammaer = new Grammar(builder);

            recognizer.LoadGrammar(grammaer);
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            #endregion

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontFamilyComboBox.ItemsSource = fontFamilies;

            List<double> fontSizes = new List<double> { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
            fontSizeComboBox.ItemsSource = fontSizes;
        }

        private void ViewModel_SelectedNoteChanged(object? sender, EventArgs e)
        {
            contentRichTextBox.Document.Blocks.Clear();
            if (viewModel.SelectedNote != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedNote.FileLocation))
                {
                    FileStream fileStream = new FileStream(viewModel.SelectedNote.FileLocation, FileMode.Open);
                    var contents = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
                    contents.Load(fileStream, DataFormats.Rtf);
                }
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);    
            if(string.IsNullOrEmpty(App.UserId))
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                viewModel.GetNotebooks();
            }
        }
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;

            contentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(recognizedText)));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Shutdown();
        }

        bool isRecogninzing = false;
        private async void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                recognizer.RecognizeAsyncStop();
            }
        }

        private void contentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountCharacters = 0;
            amountCharacters = (new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd)).Text.Length;

            statusTextBlock.Text = $"Document length: {amountCharacters} characters";
        }

        private void boldButton_Click(object sender, RoutedEventArgs e)
        {
            //two ways to make it happen.
            //var textToBold = new TextRange(contentRichTextBox.Selection.Start, contentRichTextBox.Selection.End);
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            else
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);

        }

        private void italicButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            else
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);

        }

        private void underlineButton_Click(object sender, RoutedEventArgs e)
        {

            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            else
            {
                TextDecorationCollection textDecorations;
                (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);

            }

        }

        private void contentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = contentRichTextBox.Selection.GetPropertyValue(FontWeightProperty);
            boldButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && selectedWeight.Equals(FontWeights.Bold);

            var selectedStyle = contentRichTextBox.Selection.GetPropertyValue(FontStyleProperty);
            italicButton.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && selectedStyle.Equals(FontStyles.Italic);

            var selectedTextDec = contentRichTextBox.Selection.GetPropertyValue(FontWeightProperty);
            underlineButton.IsChecked = (selectedTextDec != DependencyProperty.UnsetValue) && selectedTextDec.Equals(TextDecorations.Underline);

            fontFamilyComboBox.SelectedItem = contentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            fontSizeComboBox.Text = contentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty).ToString();


        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem != null)
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
        }

        private void fontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, $"{viewModel.SelectedNote.Id}.rtf");
            viewModel.SelectedNote.FileLocation = rtfFile;
            DatabaseHelper.Update(viewModel.SelectedNote);

            FileStream fileSteam = new FileStream(rtfFile, FileMode.Create);
            var contents = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
            contents.Save(fileSteam, DataFormats.Rtf);
        }
    }
}
