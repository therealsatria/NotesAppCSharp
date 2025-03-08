# Notes App C#

Aplikasi Notes sederhana yang dibuat menggunakan C# dengan penyimpanan SQLite.

## Deskripsi
Aplikasi ini merupakan aplikasi pencatatan (notes) berbasis command-line yang memungkinkan pengguna untuk membuat, membaca, memperbarui, dan menghapus catatan (CRUD operations).

## Teknologi yang Digunakan
- .NET 8.0
- SQLite (Microsoft.Data.Sqlite)
- CsvHelper untuk ekspor data
- Microsoft.Extensions.Configuration untuk manajemen konfigurasi

## Struktur Project
```
NotesAppCSharp/
├── src/
│   ├── Models/      # Model data
│   ├── Services/    # Layer servis untuk logika bisnis
│   └── Functions/   # Fungsi-fungsi utilitas
├── Program.cs       # Entry point aplikasi
├── notes.db        # File database SQLite
└── appsettings.json # File konfigurasi
```

## Persyaratan Sistem
- .NET 8.0 SDK atau yang lebih baru
- SQLite

## Cara Instalasi
1. Clone repository ini
2. Buka terminal dan navigasi ke direktori project
3. Jalankan perintah:
   ```bash
   dotnet restore
   dotnet build
   ```

## Cara Penggunaan
Jalankan aplikasi dengan perintah:
```bash
dotnet run
```

## Fitur
- Manajemen catatan (CRUD)
- Penyimpanan data menggunakan SQLite
- Kemampuan ekspor data ke format CSV
- Konfigurasi yang mudah melalui appsettings.json
- Enkripsi data untuk keamanan catatan

## Keamanan Data
Aplikasi ini menerapkan sistem enkripsi untuk melindungi data pengguna:
- Semua data yang disimpan dalam database SQLite dienkripsi secara otomatis
- Kunci enkripsi dikonfigurasi melalui file `appsettings.json`
- Data akan didekripsi secara otomatis saat ditampilkan menggunakan kunci yang telah ditentukan
- Pastikan untuk menjaga kerahasiaan kunci enkripsi Anda di `appsettings.json`

## Kontribusi
Silakan berkontribusi dengan membuat pull request atau melaporkan issues.

## Lisensi
MIT License
