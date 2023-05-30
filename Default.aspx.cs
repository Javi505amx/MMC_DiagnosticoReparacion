using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Threading;

namespace _2._0_DiagnosticoReparacion
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }
        public void saveDataUser()
        {
            string username;
            username = txtUsername.Text;
            Session["user"] = txtUsername.Text;
            //Session["userLogin"] = lblUser.Text;
            Session["password"] = txtPassword.Text;

            string conect = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            SqlConnection cn = new SqlConnection(conect);
            SqlCommand cmd = new SqlCommand("GetFullName", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Connection.Open();
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            sqlDataReader.Read();
            string fullName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("FullName"));
            cn.Close();
            Session["fullName"] = fullName;
        }

        protected void txtUsername_TextChanged(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        protected void LoginBtn_Click(object sender, EventArgs e)
        {
            //Conexion con la base de datos 
            string conect = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(conect);
            SqlCommand cmd = new SqlCommand("ValidateUser", sqlCon)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Connection.Open();//pasar valores a procedimiento almacenado "Validate USER"
            cmd.Parameters.Add("@User", SqlDbType.VarChar, 50).Value = txtUsername.Text;
            cmd.Parameters.Add("@Pass", SqlDbType.VarChar, 50).Value = txtPassword.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            int result = dr.GetInt32(dr.GetOrdinal("Users"));
            sqlCon.Close();
            if (result > 0)
            {//valida si usuario es admin
                SqlConnection sqlConAdmin = new SqlConnection(conect);
                SqlCommand cmdAdmin = new SqlCommand("ValidateUserAdmin", sqlConAdmin)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmdAdmin.Connection.Open();
                cmdAdmin.Parameters.Add("@User", SqlDbType.VarChar, 50).Value = txtUsername.Text;
                cmdAdmin.Parameters.Add("@Pass", SqlDbType.VarChar, 50).Value = txtPassword.Text;
                SqlDataReader drAdmin = cmdAdmin.ExecuteReader();
                drAdmin.Read();
                int resultAdmin = drAdmin.GetInt32(drAdmin.GetOrdinal("Users"));
                cmdAdmin.Connection.Close();
                Session["UserAdmin"] = resultAdmin;
                saveDataUser();
                alert.Visible = true;
                AlertIcon.Attributes.Add("class", "bi bi-bug-fill");
                alert.Attributes.Add("class", " alert alert-danger  alert-dismissible  animate__animated animate__fadeIn ");
                alertText.Text = "Incorrect User or Password";
                ClientScript.RegisterStartupScript(GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + alert.ClientID + "').style.display='none'\",4000)</script>");
                //Text = "Ingresa tus datos";
                //labelwrong.ForeColor = Color.Red;
                txtUsername.Focus();
                txtPassword.Focus();
                txtPassword.Text = null;
                txtUsername.Text = null;
                Response.Redirect("Menu.aspx");
            }
            else
            {
                alert.Visible = true;
                AlertIcon.Attributes.Add("class", "bi bi-bug-fill");
                alert.Attributes.Add("class", " alert alert-danger  alert-dismissible  animate__animated animate__fadeIn ");
                alertText.Text = "Incorrect User or Password";
                ClientScript.RegisterStartupScript(GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + alert.ClientID + "').style.display='none'\",4000)</script>");
                //Text = "Ingresa tus datos";
                //labelwrong.ForeColor = Color.Red;
                txtUsername.Focus();
                txtPassword.Focus();
                txtPassword.Text = null;
                txtUsername.Text = null;
            }
            cmd.Connection.Close();
        }
    }
}