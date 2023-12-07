using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.Storage.Pickers;
using Windows.Devices.Geolocation;

namespace Dev_natif
{
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCapture;

        public MainPage()
        {
            this.InitializeComponent();
            ShowLocationOnStartup();
        }

        // Camera
        private async void StartPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

            cameraPreview.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();

            StartPreviewButton.IsEnabled = false;
            StopPreviewButton.IsEnabled = true;
        }

        private async void StopPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaCapture != null)
            {
                await mediaCapture.StopPreviewAsync();
                cameraPreview.Source = null;
                StartPreviewButton.IsEnabled = true;
                StopPreviewButton.IsEnabled = false;
            }
        }

        // Géolocalisation
        private async void ShowLocationOnStartup()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition position = await geolocator.GetGeopositionAsync();
            double latitude = position.Coordinate.Point.Position.Latitude;
            double longitude = position.Coordinate.Point.Position.Longitude;
            LocationText.Text = $"Latitude : {latitude}, Longitude : {longitude}";
        }

        // Micro
        private async void StartAudioCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaCapture == null)
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();
            }

            if (mediaCapture != null)
            {
                var storageFile = await KnownFolders.MusicLibrary.CreateFileAsync("audio.wav", CreationCollisionOption.GenerateUniqueName);
                await mediaCapture.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateWav(AudioEncodingQuality.Auto), storageFile);

                StartAudioCaptureButton.IsEnabled = false;
                StopAudioCaptureButton.IsEnabled = true;
                StatusText.Text = "État de la capture audio : En cours d'enregistrement";
            }
        }

        private async void StopAudioCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaCapture != null)
            {
                await mediaCapture.StopRecordAsync();
                StartAudioCaptureButton.IsEnabled = true;
                StopAudioCaptureButton.IsEnabled = false;
                StatusText.Text = "État de la capture audio : Arrêtée";
            }
        }

        // Fichier
        private async void CopyFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Sélectionnez le fichier source à partir duquel vous souhaitez copier
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".jpg"); // Filtrez par extension si nécessaire

            StorageFile sourceFile = await openPicker.PickSingleFileAsync();

            if (sourceFile != null)
            {
                // Sélectionnez le dossier de destination
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                StorageFolder destinationFolder = await folderPicker.PickSingleFolderAsync();

                if (destinationFolder != null)
                {
                    try
                    {
                        // Copiez le fichier source dans le dossier de destination
                        await sourceFile.CopyAsync(destinationFolder, sourceFile.Name, NameCollisionOption.GenerateUniqueName);
                        StatusTextFichier.Text = $"Fichier copié avec succès vers : {destinationFolder.Path}";
                    }
                    catch (Exception ex)
                    {
                        StatusTextFichier.Text = $"Erreur lors de la copie du fichier : {ex.Message}";
                    }
                }
                else
                {
                    StatusTextFichier.Text = "Dossier de destination non sélectionné.";
                }
            }
            else
            {
                StatusTextFichier.Text = "Fichier source non sélectionné.";
            }
        }

    }
}





