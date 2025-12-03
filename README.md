# 🎵 MiniSpotify

**MiniSpotify** je jednoduchá webová aplikace inspirovaná Spotify.  
Umožňuje spravovat lokální hudební knihovnu – **interprety, alba, skladby a playlisty** – a přehrávat **MP3 přímo v prohlížeči** pomocí HTML5 audio přehrávače.

---

## 📚 Obsah

- [Hlavní funkce](#-hlavní-funkce)
  - [Interpreti (Artists)](#-interpreti-artists)
  - [Alba (Albums)](#-alba-albums)
  - [Skladby (Tracks)](#-skladby-tracks)
  - [Playlisty (Playlists)](#-playlisty-playlists)
  - [Vyhledávání (Search)](#-vyhledávání-search)
  - [Přehrávač](#-přehrávač)
- [Použité technologie](#-použité-technologie)
- [Struktura projektu](#-struktura-projektu)
- [Požadavky](#-požadavky)
- [Jak projekt spustit](#-jak-projekt-spustit)
- [Práce s databází](#-práce-s-databází)
- [Základní použití aplikace](#-základní-použití-aplikace)

---

## ✨ Hlavní funkce

### 👨‍🎤 Interpreti (Artists)

- Přehled všech interpretů.
- Vytváření, editace a mazání interpretů.
- Volitelné pole `Country`.

---

### 💿 Alba (Albums)

- Přehled alb, včetně vazby na interprety.
- Vytváření, editace a mazání alb.
- Nahrání obalu alba (JPEG/PNG)  
  → obaly se ukládají do složky `wwwroot/covers`.
- Detail alba s výpisem skladeb.

---

### 🎧 Skladby (Tracks)

- Přehled skladeb s informací o interpretovi a albu.
- Vytváření skladeb vázaných na konkrétní album.
- Volitelné zadání délky skladby (v sekundách).
- Nahrání MP3 souboru (pole **Audio file (mp3)**)  
  → soubory se ukládají do `wwwroot/music`.
- Editace a mazání skladeb.

---

### 🎛 Playlisty (Playlists)

- Vytváření a správa playlistů.
- Přidávání/odebírání skladeb do / z playlistu.
- Řazení skladeb v playlistu (pole `Position` v tabulce `PlaylistTrack`).
- Seznam playlistů je zobrazen v levém **sidebaru**.

---

### 🔎 Vyhledávání (Search)

Jednoduché fulltext vyhledávání podle dotazu v:

- **Interpretech**
- **Albech**
- **Skladbách**

Z výsledků lze přehrát skladby přímo v prohlížeči.

---

### ▶️ Přehrávač

HTML5 audio přehrávač v patičce (footer):

- **Play / Pause / Next / Previous**
- Posuvník průběhu skladby (**seek bar**)
- Zobrazení aktuálního času a celkové délky skladby
- Ovládání hlasitosti
- Fronta skladeb (**queue**) – ovládána JavaScriptem v `wwwroot/js/player.js`

Tlačítka **Play** u skladeb předávají do přehrávače informace o:

- zdroji (MP3 soubor),
- názvu skladby,
- interpretovi,
- obalu alba.

---

## 🛠 Použité technologie

### Back-end

- **ASP.NET Core 8.0** (MVC, Razor Views)
- **Entity Framework Core** (SQLite provider)
- `AppDbContext` s entitami:
  - `Artist`
  - `Album`
  - `Track`
  - `Playlist`
  - `PlaylistTrack`

### Databáze

- **SQLite** databáze v souboru:  
  `MiniSpotify/MiniSpotify/AppData/spotify.db`

Konfigurace připojení v `appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Data Source=AppData/spotify.db;Cache=Shared"
} 
```
## Front-end

- **Bootstrap 5**
- **Bootstrap Icons**
- **jQuery + jQuery Validation**
- **Vlastní styly** – `wwwroot/css/site.css`
- **Vlastní JS přehrávač** – `wwwroot/js/player.js`


## Struktura projektu

```text
MiniSpotify/
 ├─ MiniSpotify.sln           – řešení Visual Studio
 └─ MiniSpotify/
     ├─ Program.cs            – start aplikace, konfigurace služeb
     ├─ appsettings*.json     – konfigurace (včetně DB)
     ├─ Models/
     │   ├─ AppDbContext.cs   – EF Core DbContext
     │   ├─ Artist.cs
     │   ├─ Album.cs
     │   ├─ Track.cs
     │   ├─ Playlist.cs
     │   └─ PlaylistTrack.cs
     ├─ Controllers/
     │   ├─ HomeController.cs
     │   ├─ ArtistsController.cs
     │   ├─ AlbumsController.cs
     │   ├─ TracksController.cs
     │   ├─ PlaylistsController.cs
     │   └─ SearchController.cs
     ├─ Views/
     │   ├─ Shared/_Layout.cshtml, _SidebarPlaylists.cshtml, ...
     │   ├─ Artists/*.cshtml
     │   ├─ Albums/*.cshtml
     │   ├─ Tracks/*.cshtml
     │   ├─ Playlists/*.cshtml
     │   └─ Search/Index.cshtml
     ├─ wwwroot/
     │   ├─ css/site.css      – vzhled aplikace (dark theme)
     │   ├─ js/player.js      – logika přehrávače
     │   ├─ covers/           – obaly alb
     │   └─ music/            – nahrané MP3
     └─ AppData/
         ├─ spotify.db        – SQLite databáze
         └─ spotify.sqbpro    – projekt pro SQLiteStudio/DB Browser apod.

```
## Požadavky

- .NET **8.0 SDK** (nebo novější, kompatibilní s `net8.0`)
- Operační systém: **Windows / Linux / macOS** s podporou .NET
- Není potřeba instalovat samostatný SQLite server – používá se souborová databáze `AppData/spotify.db`


## Jak projekt spustit

### 1. Spuštění ve Visual Studiu

1. Otevřete řešení **`MiniSpotify.sln`** ve Visual Studiu.
2. Ujistěte se, že **startovací projekt** je `MiniSpotify` (webový projekt).
3. Spusťte aplikaci:
   - klávesou **F5** (debug režim), nebo  
   - **Ctrl+F5** (bez debug režimu).
4. Visual Studio spustí Kestrel / IIS Express a otevře prohlížeč, typicky na adrese  
   `http://localhost:5083` (případně dle nastavení v `Properties/launchSettings.json`).

> Databáze `AppData/spotify.db` je již součástí projektu a obsahuje ukázková data,  
> takže **není nutné spouštět migrace ani inicializaci databáze**.
## Práce s databází

- Databáze je uložená v souboru **`AppData/spotify.db`**.
- Tento soubor **nemazat**, jinak přijdete o vzorová data  
  (interpreti, alba, skladby, playlisty).

Pokud potřebujete databázi upravovat ručně:

1. Otevřete soubor **`spotify.db`** v nástroji jako je:
   - **DB Browser for SQLite**, nebo  
   - **SQLiteStudio**.
2. Schéma databáze odpovídá třídám v `Models/*`:
   - tabulky: `Artist`, `Album`, `Track`, `Playlist`, `PlaylistTrack`.


## Základní použití aplikace

### Navigace

- Horní menu obsahuje odkazy na:
  - **Artists**
  - **Albums**
  - **Tracks**
  - **Playlists**
  - **Search**
   <img width="1514" height="131" alt="image" src="https://github.com/user-attachments/assets/75f833d7-b6a1-4b86-8131-3a77a2f40fb9" />

- Levý panel (**sidebar**) zobrazuje seznam playlistů:
  - kliknutím na název playlistu přejdete na jeho **detail**.
<img width="346" height="766" alt="image" src="https://github.com/user-attachments/assets/1d1d4793-1990-4632-824b-536f42954948" />

### Přidávání dat

- **Artists → Add artist**
  <img width="1144" height="568" alt="image" src="https://github.com/user-attachments/assets/d6d5f851-7d0a-4d9f-a4a5-3e43db1c7c1b" />
  
  – vytvoření nového interpreta.
  <img width="1168" height="492" alt="image" src="https://github.com/user-attachments/assets/873d5936-4dd0-4cf6-bdb4-2d92d23de15c" />

- **Albums → Add album**
  <img width="1194" height="847" alt="image" src="https://github.com/user-attachments/assets/4b749355-a029-42ed-858f-0f1ded088f4c" />

  – vytvoření nového alba:
<img width="1146" height="610" alt="image" src="https://github.com/user-attachments/assets/24f04cb6-83c7-4584-9424-c2f6da0112f4" />

  - vyberete interpreta,
  - můžete nahrát obal alba.
- **Tracks → Add track**
  <img width="1169" height="802" alt="image" src="https://github.com/user-attachments/assets/ffc06a08-e390-480e-8100-8263db03742f" />

  – vytvoření nové skladby:
  - vyberete album,
  - můžete nahrát MP3 soubor.
  <img width="1177" height="628" alt="image" src="https://github.com/user-attachments/assets/eb667303-140b-4689-a06a-b8cd271d76f1" />


### Playlisty

- **Playlists → Add playlist**  
  – vytvoření nového playlistu.
  <img width="347" height="74" alt="image" src="https://github.com/user-attachments/assets/9ce9a8ff-6dbe-44a0-a255-252480fe16ee" />
  <img width="1148" height="557" alt="image" src="https://github.com/user-attachments/assets/fbdb2837-d953-4c42-b99b-abe5199df5a5" />

- V detailu playlistu můžete:
  - přidávat skladby přes rozbalovací seznam a tlačítko **Add**,
  - u každé skladby měnit její **pozici** v playlistu,
  - skladbu z playlistu **odebrat**.
  <img width="1155" height="845" alt="image" src="https://github.com/user-attachments/assets/bcad7ab9-577a-44af-85e9-dcab469ec9f3" />


### Přehrávání skladeb

- U skladeb a výsledků vyhledávání je tlačítko **Play**.
- Po kliknutí na **Play** se:
  - skladba přidá do **fronty přehrávače**,
  - okamžitě začne přehrávat.

Integrovaný přehrávač v patičce stránky umožňuje:
<img width="1919" height="106" alt="image" src="https://github.com/user-attachments/assets/0057eaf4-6905-43b9-8630-e5d2c7399b81" />

- **Pozastavit / pokračovat** v přehrávání,
- **Přeskočit** na další / předchozí skladbu,
- **Přetáčet skladbu** pomocí posuvníku (seek bar),
- **Upravovat hlasitost**.


