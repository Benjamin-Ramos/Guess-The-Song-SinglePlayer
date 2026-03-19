# 🎵 Guess The Song - SinglePlayer

**Guess The Song** es una aplicación de escritorio interactiva desarrollada en **C# con WPF**. El objetivo es poner a prueba tus conocimientos musicales permitiéndote buscar cualquier artista y adivinar sus canciones más populares escuchando una pequeña previa de audio.

[![GitHub Repo](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/Benjamin-Ramos/Guess-The-Song-SinglePlayer.git)

---

## 🚀 Características Principal

* **Búsqueda en Tiempo Real:** Utiliza la API de iTunes para obtener los temas más relevantes de cualquier artista que ingreses.
* **Audio Streaming:** Reproducción de fragmentos de canciones mediante la librería **NAudio**.
* **Buscador Inteligente con Autocompletado:** El ComboBox de respuestas incluye un filtro que normaliza el texto (ignora tildes, diéresis y mayúsculas) para facilitar la búsqueda de canciones.
* **Sistema de Puntuación:** Registro dinámico de aciertos y visualización de progreso.
* **Historial de Canciones (Para Debug):** Un DataGrid inferior que muestra qué canciones han pasado, cuáles adivinaste o saltaste.
* **Interfaz Moderna:** Diseño oscuro con estilos personalizados, efectos de sombra (DropShadow) y estados visuales (verde para aciertos, rojo para errores).

---

## 🛠️ Stack Tecnológico

* **Lenguaje:** C# (.NET)
* **Interfaz:** WPF (Windows Presentation Foundation)
* **Arquitectura:** MVVM (Model-View-ViewModel)
* **Librerías:** * [NAudio](https://github.com/naudio/NAudio) para la gestión de salida de audio.
    * `System.Net.Http` para el consumo de servicios REST.
* **API Externa:** iTunes Search API.

---

## 🎮 Cómo Jugar

1.  **Ingresa un Artista:** Escribe el nombre del artista en el cuadro de búsqueda y haz clic en **BUSCAR**.
2.  **Escucha:** Presiona **▶ ESCUCHAR** para reproducir la previa de la canción actual.
3.  **Selecciona tu respuesta:** Empieza a escribir el nombre de la canción en el desplegable. El sistema filtrará automáticamente las opciones.
4.  **Confirma:** Haz clic en **¡CONFIRMAR RESPUESTA!**. Si es correcta, verás la portada real del álbum y sumarás un punto.
5.  **Skip:** Si no la conoces, puedes usar el botón **➔ SKIP**, pero se te llamará "inútil" y se revelará la respuesta correcta.

---

## 📁 Estructura del Código

El proyecto sigue una estructura organizada para mantener la separación de responsabilidades:

* **`Models/`**: Definición de los datos de la canción (`Result`) y la raíz de la respuesta de la API.
* **`ViewModels/`**: `GameViewModel.cs` contiene toda la lógica de control del juego, manejo de colecciones y estados de la UI.
* **`Services/`**:
    * `MusicService.cs`: Encargado de las peticiones a la API y la limpieza inteligente de títulos (remoción de "feat.", "Remix", etc.).
    * `AudioService.cs`: Gestión del motor de audio (Play/Stop).
* **`Views/`**: `MainWindow.xaml` y diccionarios de recursos para el diseño visual.

---

## 🔧 Instalación y Configuración

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/Benjamin-Ramos/Guess-The-Song-SinglePlayer.git
    ```
2.  **Abrir en Visual Studio:** Abre el archivo `.sln`.
3.  **Restaurar Paquetes NuGet:** Asegúrate de que `NAudio` esté correctamente instalado.
4.  **Ejecutar:** Presiona `F5` o el botón de Iniciar.

---

## 📝 Notas de Desarrollo

* **Normalización:** Se implementó un método de normalización `FormD` para que caracteres como `á` sean tratados como `a`, mejorando la precisión de la búsqueda manual en el juego.
* **Limpieza de Títulos:** El servicio de música utiliza expresiones regulares (Regex) para limpiar los títulos de las canciones que vienen de iTunes, eliminando etiquetas como `(feat. ...)` para que el usuario no tenga que escribir nombres excesivamente largos.

---

## 📄 Licencia
Este proyecto es de código abierto bajo la licencia [MIT](LICENSE).

---

**Desarrollado por [Benjamin Ramos](https://github.com/Benjamin-Ramos)**
