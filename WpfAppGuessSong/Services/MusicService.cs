using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using WpfAppGuessSong.Models;

namespace WpfAppGuessSong.Services
{
    public class MusicService
    {
        private readonly HttpClient _client = new HttpClient();

        public string LimpiarTituloInteligente(string titulo)
        {
            if (string.IsNullOrEmpty(titulo)) return titulo;
            titulo = Regex.Replace(titulo, @"[\(\[][^\]\)]*(feat\.|feat|with|presents|ft\.)[^\]\)]*[\)\]]", "", RegexOptions.IgnoreCase);
            titulo = Regex.Replace(titulo, @"\[feat\.[^\]]*\]", "", RegexOptions.IgnoreCase);
            titulo = titulo.Replace("()", "").Replace("[]", "");

            if (titulo.Contains("]"))
            {
                int indiceCierre = titulo.LastIndexOf(']');
                if (indiceCierre < titulo.Length - 1)
                {
                    titulo = titulo.Substring(0, indiceCierre + 1);
                }
            }

            titulo = Regex.Replace(titulo, @"\s+", " ").Trim();
            return titulo;
        }

        public async Task<List<Result>> GetSongsAsync(string artistName)
        {
            string termEscaped = Uri.EscapeDataString(artistName.Trim());
            string urlArtista = $"https://itunes.apple.com/search?term={termEscaped}&entity=musicArtist&attribute=artistTerm&limit=1";

            try
            {
                var resArtista = await _client.GetFromJsonAsync<Rootobject>(urlArtista);

                var artistaData = resArtista?.results
                    .FirstOrDefault(a => a.artistName.Equals(artistName.Trim(), StringComparison.OrdinalIgnoreCase))
                    ?? resArtista?.results.FirstOrDefault();

                if (artistaData == null) return new List<Result>();

                string urlCanciones = $"https://itunes.apple.com/lookup?id={artistaData.artistId}&entity=song&limit=50";
                var resCanciones = await _client.GetFromJsonAsync<Rootobject>(urlCanciones);

                var cancionesLimpias = resCanciones?.results
                    .Where(r => r.kind == "song" && !string.IsNullOrEmpty(r.previewUrl))
                    .Select(r => {
                        r.trackName = LimpiarTituloInteligente(r.trackName);
                        return r;
                    })
                    .GroupBy(r => r.trackName.ToLower())
                    .Select(g => g.First())
                    .ToList();

                return cancionesLimpias;
            }
            catch { return new List<Result>(); }
        }
    }
}