using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;

namespace Doviz_Ofisi
{
    public partial class Form1 : Form
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-2HOS6DT\SQLEXPRESS;Initial Catalog=DbDoviz;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select MIKTAR From TBLDOLAR", baglanti);
            SqlCommand komut1 = new SqlCommand("Select MIKTAR From TBLTL", baglanti);
            SqlCommand komut2 = new SqlCommand("Select MIKTAR From TBLEURO", baglanti);
            lblDolarKalan.Text = komut.ExecuteScalar().ToString();
            lblTlKalan.Text = komut1.ExecuteScalar().ToString();
            lblEuroKalan.Text = komut2.ExecuteScalar().ToString();
            baglanti.Close();

            string bugun = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var xmlDosya = new XmlDocument();
            xmlDosya.Load(bugun);

            string dolaralis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteBuying").InnerXml;
            lblDolarAlis.Text = dolaralis;

            string dolarsatis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml;
            lblDolarSatis.Text = dolarsatis;

            string euroalis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteBuying").InnerXml;
            lblEuroAlis.Text = euroalis;
            string eurosatis = xmlDosya.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml;
            lblEuroSatis.Text = eurosatis;
        }

        private void btnDolarAl_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarAlis.Text;
            btnSatisYap.Text = "$ --> ₺";
            lblTutar.Text = "₺";
            lblMiktar.Text = "$";
        }

        private void btnDolarSat_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarSatis.Text;
            btnSatisYap2.Text = "₺ --> $";
            lblTutar.Text = "$";
            lblMiktar.Text = "₺";
        }

        private void btnEuroAl_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroAlis.Text;
            btnSatisYap.Text = "€ --> ₺";
            lblMiktar.Text = "€";
            lblTutar.Text = "₺";
        }

        private void btnEuroSat_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroSatis.Text;
            btnSatisYap2.Text = "₺ --> €";
            lblTutar.Text = "€";
            lblMiktar.Text = "₺";
        }

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            try
            {
                double kur, miktar, tutar;
                kur = Convert.ToDouble(txtKur.Text);
                miktar = Convert.ToDouble(txtMiktar.Text);
                tutar = kur * miktar;
                txtTutar.Text = tutar.ToString();
                txtKalan.Text = "";
            }
            catch
            {
                MessageBox.Show("İlgili alanları doldurunuz");
            }

        }

        private void txtKur_TextChanged(object sender, EventArgs e)
        {
            txtKur.Text = txtKur.Text.Replace(".", ",");
        }

        private void btnSatisYap2_Click(object sender, EventArgs e)
        {
            try
            {
                double kur = Convert.ToDouble(txtKur.Text);
                int miktar = Convert.ToInt32(txtMiktar.Text);
                int tutar = Convert.ToInt32(miktar / kur);
                txtTutar.Text = tutar.ToString();
                double kalan;
                kalan = miktar % kur;
                txtKalan.Text = kalan.ToString();
            }
            catch
            {
                MessageBox.Show("İlgili alanları doldurunuz","Bilgi", MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }


        }

        private void btnSatisYap2_MouseHover(object sender, EventArgs e)
        {
            lblOnizle1.Text = "Önizle";
        }

        private void btnSatisYap2_MouseLeave(object sender, EventArgs e)
        {
            lblOnizle1.Text = "";
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtKur.Text = "";
            txtMiktar.Text = "";
            txtTutar.Text = "";
            txtKalan.Text = "";
            lblTutar.Text = "";
            lblMiktar.Text = "";
            btnSatisYap.Text = "İşlem 2";
            btnSatisYap2.Text = "İşlem 1";
        }

        private void lblDolarKalan_Click(object sender, EventArgs e)
        {

        }

        private void btnDolar_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select MIKTAR From TBLDOLAR", baglanti);
            lblDolarKalan.Text = komut.ExecuteScalar().ToString();
            baglanti.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select MIKTAR From TBLEURO", baglanti);
            lblEuroKalan.Text = komut.ExecuteScalar().ToString();
            baglanti.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select MIKTAR From TBLTL", baglanti);
            lblTlKalan.Text = komut.ExecuteScalar().ToString();
            baglanti.Close();
        }

        private void btnAl_Click(object sender, EventArgs e)
        {
            double guncelleDolar, guncelleTL, guncelleEuro;

            try
            {

                if (lblTutar.Text == "$")
                {
                    baglanti.Open();
                    guncelleDolar = Convert.ToDouble(lblDolarKalan.Text) - Convert.ToDouble(txtTutar.Text);
                    guncelleTL = Convert.ToDouble(lblTlKalan.Text) + Convert.ToDouble(txtMiktar.Text);
                    SqlCommand komut = new SqlCommand("update TBLDOLAR set MIKTAR=@p1", baglanti);
                    SqlCommand komut1 = new SqlCommand("update TBLTL set MIKTAR=@p2", baglanti);
                    komut.Parameters.AddWithValue("@p1", guncelleDolar);
                    komut1.Parameters.AddWithValue("@p2", guncelleTL);
                    komut.ExecuteNonQuery();
                    komut1.ExecuteNonQuery();
                    MessageBox.Show("İşleminiz tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    baglanti.Close();

                }
            }
            catch
            {
                MessageBox.Show("Bir şeyler ters gitti", "Bilgi", MessageBoxButtons.RetryCancel);
            }
            try
            {
                if (lblTutar.Text == "€")
                {
                    baglanti.Open();
                    guncelleEuro = Convert.ToDouble(lblEuroKalan.Text) - Convert.ToDouble(txtTutar.Text);
                    guncelleTL = Convert.ToDouble(lblTlKalan.Text) + Convert.ToDouble(txtMiktar.Text);
                    SqlCommand komut = new SqlCommand("update TBLEURO set MIKTAR=@p1", baglanti);
                    SqlCommand komut1 = new SqlCommand("update TBLTL set MIKTAR=@p2", baglanti);
                    komut.Parameters.AddWithValue("@p1", guncelleEuro);
                    komut1.Parameters.AddWithValue("@p2", guncelleTL);
                    komut.ExecuteNonQuery();
                    komut1.ExecuteNonQuery();
                    MessageBox.Show("İşleminiz tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    baglanti.Close();

                }
            }
            catch
            {
                MessageBox.Show("Bir şeyler ters gitti", "Bilgi", MessageBoxButtons.RetryCancel);
            }


        }

        private void btnSat_Click(object sender, EventArgs e)
        {
            double guncelleDolar, guncelleTL, guncelleEuro;

            try
            {
                if (lblMiktar.Text == "$")
                {
                    baglanti.Open();
                    guncelleDolar = Convert.ToDouble(lblDolarKalan.Text) + Convert.ToDouble(txtTutar.Text);
                    guncelleTL = Convert.ToDouble(lblTlKalan.Text) - Convert.ToDouble(txtMiktar.Text);
                    SqlCommand komut = new SqlCommand("update TBLDOLAR set MIKTAR=@p1", baglanti);
                    SqlCommand komut1 = new SqlCommand("update TBLTL set MIKTAR=@p2", baglanti);
                    komut.Parameters.AddWithValue("@p1", guncelleDolar);
                    komut1.Parameters.AddWithValue("@p2", guncelleTL);
                    komut.ExecuteNonQuery();
                    komut1.ExecuteNonQuery();
                    MessageBox.Show("İşleminiz tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    baglanti.Close();
                }
            }
            catch
            {
                MessageBox.Show("Bir şeyler ters gitti", "Bilgi", MessageBoxButtons.RetryCancel);
            }
            try
            {
                if (lblMiktar.Text == "€")
                {
                    baglanti.Open();
                    guncelleEuro = Convert.ToDouble(lblEuroKalan.Text) + Convert.ToDouble(txtTutar.Text);
                    guncelleTL = Convert.ToDouble(lblTlKalan.Text) - Convert.ToDouble(txtMiktar.Text);
                    SqlCommand komut = new SqlCommand("update TBLEURO set MIKTAR=@p1", baglanti);
                    SqlCommand komut1 = new SqlCommand("update TBLTL set MIKTAR=@p2", baglanti);
                    komut.Parameters.AddWithValue("@p1", guncelleEuro);
                    komut1.Parameters.AddWithValue("@p2", guncelleTL);
                    komut.ExecuteNonQuery();
                    komut1.ExecuteNonQuery();
                    MessageBox.Show("İşleminiz tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    baglanti.Close();
                }
            }
            catch
            {
                MessageBox.Show("Bir şeyler ters gitti","Bilgi",MessageBoxButtons.RetryCancel);
            }


        }

        private void btnSatisYap_MouseHover(object sender, EventArgs e)
        {
            lblOnizle2.Text = "Önizle";
        }

        private void btnSatisYap_MouseLeave(object sender, EventArgs e)
        {
            lblOnizle2.Text = "";
        }
    }
}
