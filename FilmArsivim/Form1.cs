using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;

namespace FilmArsivim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // ESC tuşunu form düzeyinde yakalayalım
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private bool _wvTamEkran = false;
        private Control _oncekiParent;
        private DockStyle _oncekiDock;
        private Rectangle _oncekiBounds;

        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-6O7H141\\SQLEXPRESS;Initial Catalog=dbFilmArsivi;Integrated Security=True");

        private void FilmListele()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("select * from tblFilmler", baglanti);
            da.Fill(dt);
            dgvFilmListesi.DataSource = dt;
        }

        private void FormTemizle()
        {
            txtFilmAdi.Text = string.Empty;
            txtKategori.Text = string.Empty;
            txtLink.Text = string.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FilmListele();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into tblFilmler (Ad, Kategori, Link) values (@p1, @p2, @p3)", baglanti);
            komut.Parameters.AddWithValue("@p1", txtFilmAdi.Text);
            komut.Parameters.AddWithValue("@p2", txtKategori.Text);
            komut.Parameters.AddWithValue("@p3", txtLink.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            FilmListele();
            FormTemizle();
        }

        private void btnTamEkran_Click(object sender, EventArgs e)
        {
            if (!_wvTamEkran)
            {
                // 1) Konum / boyut bilgisini sakla
                _oncekiParent = webView21.Parent;
                _oncekiDock = webView21.Dock;
                _oncekiBounds = webView21.Bounds;

                // 2) WebView2'yi doğrudan formun üzerine al ve doldur
                this.Controls.Add(webView21);
                webView21.BringToFront();
                webView21.Dock = DockStyle.Fill;

                _wvTamEkran = true;
            }
            else
            {
                // 1) Eski parent'a geri koy
                _oncekiParent.Controls.Add(webView21);

                // 2) Dock / boyutları geri yükle
                webView21.Dock = _oncekiDock;
                webView21.Bounds = _oncekiBounds;

                _wvTamEkran = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _wvTamEkran)
            {
                btnTamEkran_Click(null, null); // geri dön
                e.Handled = true;
            }
        }

        private void btnHakkimizda_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bu proje İlyas Parlak tarafından 19.05.2025 tarihinde oluşturulmuştur.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Rastgele için tek Random nesnesi
        private readonly Random _rnd = new Random();

        private readonly Color[] _renkPaleti =
        {
            Color.LightSkyBlue,
            Color.LightGreen,
            Color.Khaki,
            Color.Plum,
            Color.Coral,
            Color.LightSalmon,
            Color.MediumAquamarine,
            Color.PaleTurquoise,
            Color.Moccasin,
            Color.Thistle
        };

        private void btnRenkDegistir_Click(object sender, EventArgs e)
        {
            int i = _rnd.Next(_renkPaleti.Length);
            this.BackColor = _renkPaleti[i];
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dgvFilmListesi_DoubleClick(object sender, EventArgs e)
        {
            int secilen = dgvFilmListesi.SelectedCells[0].RowIndex;
            string link = dgvFilmListesi.Rows[secilen].Cells[3].Value.ToString();
            webView21.Source = new Uri(link);
        }
    }
}
