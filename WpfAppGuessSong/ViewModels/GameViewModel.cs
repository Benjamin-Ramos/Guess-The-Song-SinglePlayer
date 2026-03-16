using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfAppGuessSong.Models;
using WpfAppGuessSong.Services;

namespace WpfAppGuessSong.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly MusicService _musicService = new MusicService();
        private readonly AudioService _audioService = new AudioService();

        public ObservableCollection<Result> ListaCanciones { get; set; } = new ObservableCollection<Result>();
        public ObservableCollection<Result> CancionesRestantes { get; set; } = new ObservableCollection<Result>();

        private string _fotoArtista;
        public string FotoArtista
        {
            get => _fotoArtista;
            set { _fotoArtista = value; OnPropertyChanged(); }
        }

        private bool _mostrarPortadaReal;
        public bool MostrarPortadaReal
        {
            get => _mostrarPortadaReal;
            set { _mostrarPortadaReal = value; OnPropertyChanged(); }
        }

        private Result _cancionActual;
        public Result CancionActual
        {
            get => _cancionActual;
            set
            {
                _cancionActual = value;
                if (_cancionActual != null && !string.IsNullOrEmpty(_cancionActual.artworkUrl100))
                {
                    _cancionActual.artworkUrl100 = _cancionActual.artworkUrl100.Replace("100x100", "600x600");
                }
                OnPropertyChanged();
            }
        }

        private string _mensaje;
        public string Mensaje
        {
            get => _mensaje;
            set { _mensaje = value; OnPropertyChanged(); }
        }

        private string _artistaABuscar;
        public string ArtistaABuscar
        {
            get => _artistaABuscar;
            set { _artistaABuscar = value; OnPropertyChanged(); }
        }

        private bool _juegoActivo;
        public bool JuegoActivo
        {
            get => _juegoActivo;
            set { _juegoActivo = value; OnPropertyChanged(); }
        }

        private int _aciertos;
        public int Aciertos
        {
            get => _aciertos;
            set { _aciertos = value; OnPropertyChanged(); }
        }

        private bool _fueCorrecto;
        public bool FueCorrecto
        {
            get => _fueCorrecto;
            set { _fueCorrecto = value; OnPropertyChanged(); }
        }

        public async Task BuscarYEmpezar()
        {
            if (string.IsNullOrWhiteSpace(ArtistaABuscar)) return;

            try
            {
                _audioService.Stop();
                JuegoActivo = false;
                Aciertos = 0;
                FueCorrecto = false;

                CancionActual = null;
                MostrarPortadaReal = false;
                FotoArtista = null;

                Mensaje = "Buscando artista oficial...";

                var resultados = await _musicService.GetSongsAsync(ArtistaABuscar);

                if (resultados != null && resultados.Count >= 5)
                {
                    ListaCanciones.Clear();
                    var cancionesUnicas = resultados
                        .GroupBy(c => {
                            string nombre = c.trackName.ToLower();
                            if (nombre.Contains("(")) nombre = nombre.Split('(')[0];
                            if (nombre.Contains("-")) nombre = nombre.Split('-')[0];
                            nombre = new string(nombre.Normalize(System.Text.NormalizationForm.FormD)
                                .Where(ch => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                                .ToArray());
                            return nombre.Replace(",", "").Replace(".", "").Replace(" ", "").Trim();
                        })
                        .Select(g => g.First())
                        .ToList();

                    foreach (var c in cancionesUnicas) ListaCanciones.Add(c);

                    var cancionSolista = resultados.FirstOrDefault(r =>
                        r.artistName.Equals(ArtistaABuscar, StringComparison.OrdinalIgnoreCase));

                    string urlPerfilArtista = cancionSolista?.artistViewUrl ?? resultados[0].artistViewUrl;

                    if (!string.IsNullOrEmpty(urlPerfilArtista))
                    {
                        using (var client = new System.Net.Http.HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                            var html = await client.GetStringAsync(urlPerfilArtista);

                            string patron = "<meta property=\"og:image\" content=\"([^\"]+)\"";
                            var match = System.Text.RegularExpressions.Regex.Match(html, patron);

                            if (match.Success)
                            {
                                FotoArtista = match.Groups[1].Value;
                            }
                            else
                            {
                                FotoArtista = (cancionSolista ?? resultados[0]).artworkUrl100.Replace("100x100", "600x600");
                            }
                        }
                    }
                    else
                    {
                        FotoArtista = (cancionSolista ?? resultados[0]).artworkUrl100.Replace("100x100", "600x600");
                    }

                    await Task.Delay(400);
                    ActualizarCombo();
                    JuegoActivo = true;
                    SiguienteRonda();
                }
                else
                {
                    Mensaje = "No se encontraron suficientes canciones oficiales.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Error: " + ex.Message;
            }
        }

        public void ActualizarCombo()
        {
            CancionesRestantes.Clear();
            var disponibles = ListaCanciones.Where(c => !c.FueAdivinada).OrderBy(c => c.trackName);
            foreach (var c in disponibles)
            {
                CancionesRestantes.Add(c);
            }
        }

        public async Task SkipCancion()
        {
            if (CancionActual != null)
            {
                Mensaje = $"Eres un inútil, la canción era: {CancionActual.trackName}";
                CancionActual.FueAdivinada = true;
                JuegoActivo = false;
                await Task.Delay(3000);
                ActualizarCombo();
                SiguienteRonda();
                JuegoActivo = true;
            }
        }

        public void SiguienteRonda()
        {
            _audioService.Stop();
            FueCorrecto = false;
            MostrarPortadaReal = false;

            var disponibles = ListaCanciones.Where(c => !c.FueAdivinada).ToList();

            if (disponibles.Any())
            {
                Random rnd = new Random();
                CancionActual = disponibles[rnd.Next(disponibles.Count)];
                Mensaje = $"¡A por la siguiente! Quedan {disponibles.Count}";
            }
            else
            {
                Mensaje = $"🏆 ¡JUEGO TERMINADO! Has adivinado {Aciertos} de {ListaCanciones.Count} canciones.";

                JuegoActivo = false;
                CancionActual = null;
            }
        }

        public void Reproducir() => _audioService.Play(CancionActual?.previewUrl);
        public void Detener() => _audioService.Stop();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}