﻿using ServerLega.InterfacceController;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using ServerLega.Dominio;


namespace ServerLega.Controller
{
    /// <summary>
    /// Descrizione di riepilogo per GestioneUtenteController
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente. 
    // [System.Web.Script.Services.ScriptService]
    public class GestioneUtenteController : System.Web.Services.WebService, InterfacceController.IGestioneUtenteController
    {
        public GestioneUtenteController() { }


        public Lega CreaLega(String nome, int numeroPartecipanti, String nomeSquadra, Utente utente)
        {

            SqlConnection conn = null;
            try
            {
                Lega lega = new Lega(nome, numeroPartecipanti);
                Squadra squadraAdmin = new Squadra();
                squadraAdmin.Nome = nomeSquadra;
                squadraAdmin.Utente = utente;
                lega.SquadraAdmin = squadraAdmin;
                lega.Squadre.Add(squadraAdmin);

                //CAMBIARE IL PATH A SECONDA DEL DB USATO!!
                //JACOPO
                //conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Jacopo\Source\Repos\progettoIngegneriaDelSoftware\MyFantalega\ServerLega\App_Data\DBMyFantalegaJacopo.mdf;Integrated Security=True");
                //LORENZO
                conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lorenzo\source\repos\progettoIngegneriaDelSoftware\MyFantalega\ServerLega\App_Data\DBMyFantalegaLori.mdf;Integrated Security=True");
                //ALAN
                //conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Alan\Documents\universita\terzo anno\secondo semestre\progetto\MyFantalega\ServerLega\App_Data\DBMyFantalega.mdf;Integrated Security=True");
                conn.Open();

                //QUERY DI RECUPERO DELL' USERNAME DELL'UTENTE
                SqlCommand selectUsername = new SqlCommand("SELECT * FROM Utente U WHERE U.[e-mail] = '" + utente.Email + "'", conn);
                SqlDataReader reader = selectUsername.ExecuteReader();
                String username = null;
                while (reader.Read())
                {
                    username = reader["username"].ToString();
                }
                conn.Close();
                

                //QUERY DI INSERIMENTO DELLA LEGA
                conn.Open();
                SqlCommand insertLega = new SqlCommand("INSERT INTO Lega (nome, numSquadreTot, creditiIniziali, numPor, numDif, numCen, numAtt, squadraAdmin, lista, legaAdmin ) VALUES ( '" + lega.NomeLega + "', " + lega.NumeroSquadreTotali + ", " + lega.CreditiInizialiSquadra + ", " + lega.NumeroPor + ", " + lega.NumeroDif + ", " + lega.NumeroCen + ", " + lega.NumeroAtt + ", NULL, NULL, '" + lega.NomeLega + "' )", conn);
                insertLega.ExecuteNonQuery();
                conn.Close();

                //QUERY DI INSERIMENTO DELLA SQUADRA
                conn.Open();
                SqlCommand insertSquadra = new SqlCommand("INSERT INTO Squadra (nome, creditiResidui, lega, utente ) VALUES ( '" + squadraAdmin.Nome + "', " + squadraAdmin.CreditiResidui + ", '" + lega.NomeLega + "', '" + username + "')", conn);
                insertSquadra.ExecuteNonQuery();
                conn.Close();

                //AGGIUNGO ALLA LEGA NEL DB IL NOME DELLA SQUADRA ADMIN
                conn.Open();
                SqlCommand updateLega = new SqlCommand("UPDATE Lega SET [squadraAdmin]='" + squadraAdmin.Nome + "' WHERE [nome]='" + lega.NomeLega+"'", conn);
                updateLega.ExecuteNonQuery();
                conn.Close();

                return lega;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        [WebMethod]
        public Boolean CambiaPassword(String vecchiaPass, String nuovaPass, String domanda, String risposta, Utente utente)
        {
            String usernameInFile;
            String passwordInFile;
            String emailInFile;
            String domandaInFile;
            String rispostaInFile;
            String line;

            try
            {
                //JACOPO
                //StreamWriter writer = new StreamWriter(@"C: \Users\Jacopo\Source\Repos\progettoIngegneriaDelSoftware\MyFantalega\ServerLogin\registrazioni.txt");
                //StreamReader reader = new StreamReader(@"C: \Users\Jacopo\Source\Repos\progettoIngegneriaDelSoftware\MyFantalega\ServerLogin\registrazioni.txt");
                //ALAN
                StreamWriter writer = new StreamWriter(@"C:\Users\Alan\Documents\universita\terzo anno\secondo semestre\progetto\MyFantalega\ServerLogin\registrazioni.txt");
                StreamReader reader = new StreamReader(@"C:\Users\Alan\Documents\universita\terzo anno\secondo semestre\progetto\MyFantalega\ServerLogin\registrazioni.txt");
                while ((line = reader.ReadLine()) != null)
                {
                    char[] seps = { ':' };
                    String[] values = line.Split(seps);
                    usernameInFile = values[0];
                    passwordInFile = values[1];
                    emailInFile = values[2];
                    domandaInFile = values[3];
                    rispostaInFile = values[4];
                    if (emailInFile.Equals(utente.Email) && passwordInFile.Equals(vecchiaPass) && domandaInFile.Equals(domanda) && rispostaInFile.Equals(risposta))
                    {
                        String lineToWrite = usernameInFile + ":" + nuovaPass + ":" + emailInFile + ":" + domandaInFile + ":" + rispostaInFile;
                        writer.WriteLine(lineToWrite);
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return false;
        }


        [WebMethod]
        public List<Lega> GetLeghe(Utente utente)
        {
            List<Lega> leghe = new List<Lega>();
            SqlConnection conn = null;
            try
            {   //CAMBIARE IL PATH A SECONDA DEL DB USATO!!
                ///JACOPO
                //conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Jacopo\Source\Repos\progettoIngegneriaDelSoftware\MyFantalega\ServerLega\App_Data\DBMyFantalegaJacopo.mdf;Integrated Security=True");
                //LORENZO
                conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lorenzo\Source\Repos\progettoIngegneriaDelSoftware\MyFantalega\ServerLega\App_Data\DBMyFantalegaLori.mdf;Integrated Security=True");
                //ALAN
                //conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Alan\Documents\universita\terzo anno\secondo semestre\progetto\MyFantalega\ServerLega\App_Data\DBMyFantalega.mdf;Integrated Security=True");

                conn.Open();
                //QUERY CHE RESTITUISCE LE LEGHE
                SqlCommand select = new SqlCommand("SELECT * FROM Lega WHERE nome IN (SELECT S.lega FROM Squadra S JOIN Utente U ON U.username = S.utente WHERE U.[e-mail]='" + utente.Email + "')", conn);
                SqlDataReader reader = select.ExecuteReader();

                while (reader.Read())
                {
                    Lega lega = new Lega(reader["nome"].ToString(), (int)reader.GetValue(1));
                    lega.CreditiInizialiSquadra = reader.GetInt32(2);
                    lega.NumeroPor = reader.GetInt32(3);
                    lega.NumeroDif = reader.GetInt32(4);
                    lega.NumeroCen = reader.GetInt32(5);
                    lega.NumeroAtt = reader.GetInt32(6);
                    Squadra squadraAdmin = new Squadra();
                    squadraAdmin.Nome = reader.GetString(7);
                    ListaSvincolati lista = null;
                    if (reader.GetValue(8).GetType() != typeof(DBNull) ) {
                        lista = new ListaSvincolati();
                        lista.IdLista = reader.GetInt32(8);
                    }
                    lega.ListaSvincolati = lista;
                    //squadraAdmin.Lega = lega;
                    squadraAdmin.Utente = utente;
                    lega.SquadraAdmin = squadraAdmin;

                    leghe.Add(lega);
                }

                reader.Close();

                return leghe;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        [WebMethod]
        public Boolean UniscitiAMercato(Mercato mercato, Squadra squadra)
        {
            if (mercato.Squadre.Contains(squadra))
                return false;
            return true;

        }

    }
}