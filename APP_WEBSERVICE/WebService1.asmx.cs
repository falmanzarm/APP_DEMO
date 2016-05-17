using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace APP_WEBSERVICE
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {                
        [WebMethod]
        public DataSet GetData(string query, string TableName)
        {            
            DataSet ReturnData = new DataSet();
            string stringConnection = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(stringConnection))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;

                connection.Open();                
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ReturnData, TableName);                
            }

            return ReturnData;
        }

        [WebMethod]
        public DataSet GetDataByStoreProcedure(string StoreProcedureName)
        {
            DataSet ReturnData = new DataSet();
            string stringConnection = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(stringConnection))
            {
                SqlCommand cmd = new SqlCommand(StoreProcedureName, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ReturnData, StoreProcedureName);                          
            }

            return ReturnData;
        }

        [WebMethod]
        public DataSet GetDataByStoreProcedureWithParameters(string StoreProcedureName, 
            string[] ParametersName, object[] ParametersValue)
        {
            DataSet ReturnData = new DataSet();
            string stringConnection = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;            

            using (SqlConnection connection = new SqlConnection(stringConnection))
            {
                SqlCommand cmd = new SqlCommand(StoreProcedureName, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                for(int index = 0; index < ParametersValue.Length; index ++)
                { 
                    cmd.Parameters.AddWithValue(ParametersName[index], ParametersValue[index]);                    
                }
                
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ReturnData, StoreProcedureName);
            }

            return ReturnData;
        }
    }
}
