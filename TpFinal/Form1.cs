using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using ConexionDB;

namespace TpFinal
{
    public partial class frmStock: Form
    {
        private List<Articulo> listaArticulo;
        public frmStock()
        {
            InitializeComponent();
        }

        private void frmStock_Load(object sender, EventArgs e)
        {
            
            cargar();

            cbxCampo.Items.Add("Nombre");
            cbxCampo.Items.Add("Codigo");
            cbxCampo.Items.Add("Precio");
        }

        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbxCampo.SelectedIndex.ToString();
            if(opcion == "2")
            {
                cbxCriterio.Items.Clear();
                cbxCriterio.Items.Add("Mayor a");
                cbxCriterio.Items.Add("Menor a");
                cbxCriterio.Items.Add("Igual a");
            }
            else
            {
                cbxCriterio.Items.Clear();
                cbxCriterio.Items.Add("Comienza con");
                cbxCriterio.Items.Add("Termina con");
                cbxCriterio.Items.Add("Contiene");
            }
        }

        private void dgvStock_SelectionChanged(object sender, EventArgs e)
        {
            Articulo seleccionado = (Articulo)dgvStock.CurrentRow.DataBoundItem;
            cargarImagen(seleccionado.ImagenUrl);
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxStock.Load(imagen);
            }
            catch (Exception ex)
            {

                pbxStock.Load("https://ih1.redbubble.net/image.4905811472.8675/bg,f8f8f8-flat,750x,075,f-pad,750x1000,f8f8f8.jpg");
            }
        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            listaArticulo = negocio.listar();
            dgvStock.DataSource = listaArticulo;
            dgvStock.Columns["ImagenUrl"].Visible = false;
            dgvStock.Columns["Id"].Visible = false;
            cargarImagen(listaArticulo[0].ImagenUrl);

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregar agregar = new frmAgregar();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            if (dgvStock.CurrentRow == null)
            {
                MessageBox.Show("No hay un articulo seleccionado para modificar");
                return;
            }
            seleccionado = (Articulo)dgvStock.CurrentRow.DataBoundItem;
            
            frmAgregar modificar = new frmAgregar(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            if (dgvStock.CurrentRow == null)
            {
                MessageBox.Show("No hay un articulo seleccionado para eliminar");
                return;
            }
            try
            {
                DialogResult respuesta = MessageBox.Show("¡¡ESTAS A PUNTO DE ELIMINAR UN ARTICULO!! ¿Estas seguro?", "Eliminar?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvStock.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                    cargar();
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        
        private bool validarFiltro()
        {
            if (cbxCampo.SelectedIndex == -1 || cbxCriterio.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor seleccione los parametros para filtrar!");
                return true;
            }
            if (string.IsNullOrEmpty(txtFiltro.Text))
            {
                MessageBox.Show("Por favor ingrese el parametro para filtrar en la caja de texto o use el boton 'Limpiar filtro' para ver la lista completa!");
                return true;
            }
            if (cbxCampo.SelectedItem.ToString() == "Precio")
            {
                if (!(soloNumeros(txtFiltro.Text))){
                    MessageBox.Show("Ingrese solo numeros si quiere filtrar por precios!");
                    return true;
                }
                
            }
                

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cbxCampo.SelectedItem.ToString();
                string criterio = cbxCriterio.SelectedItem.ToString();
                string filtro = txtFiltro.Text;
                dgvStock.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cargar();
        }
    }
}
