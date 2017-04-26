using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace

namespace R
{
    public class RmySqlObject
    {
        private readonly bool _event;
        private MySqlCommand _cmd;
        private MySqlConnection _connection;
        private MySqlDataReader _dataReader;
        private MySqlTransaction _transaction;

        public RmySqlObject()
        {
            MgErreur = "Constructeur";
            Host = "localhost";
            Bdd = "test";
            Pseudo = "somar";
            Password = "logitech03";
            Initialize();
        }

        /// <summary>
        ///     Initialisation d'une nouvelle instance de RmySql
        /// </summary>
        /// <param name="host">Serveur</param>
        /// <param name="bdd">Le nom de la BDD</param>
        /// <param name="pseudo">Pseudo de connexion</param>
        /// <param name="password">Mot de passe de connexion</param>
        /// <param name="checkEvent">Autorise les events</param>
        public RmySqlObject(string host, string bdd, string pseudo, string password, bool checkEvent = false)
        {
            MgErreur = "Constructeur";
            Host = host;
            Pseudo = pseudo;
            Password = password;
            Bdd = bdd;
            _event = checkEvent;
            Initialize();
        }

        public string Host { get; set; }
        public string Bdd { get; set; }
        public string Pseudo { get; set; }
        public string Password { get; set; }
        public string MgErreur { get; set; }
        public bool CheckConnexion { get; set; }

        //Gestion des Events
        public event Action<object> EventAdd;
        public event Action<object> EventUpdate;
        public event Action<object> EventDelete;
        public event Action<object> EventSelect;
        public event Action<object> EventCreateTable;
        public event Action<string> EventCreateDatabase;
        public event Action<object> EventUpdateTable;
        public event Action<object> EventDeleteTable;
        public event Action<object> EventCloseConnexion;

        /// <summary>
        ///     Event qui se déclenche lors de la fermeture de la connexion à la bdd
        /// </summary>
        protected virtual void OnEventCloseConnexion(object obj)
        {
            Action<object> handler = EventCloseConnexion;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors de la création d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet qui correspond à la table créée</param>
        protected virtual void OnEventCreateTable<T>(T obj)
        {
            Action<object> handler = EventCreateTable;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors de la modification d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet qui correspond à la table modifiée</param>
        protected virtual void OnEventUpdateTable<T>(T obj)
        {
            Action<object> handler = EventUpdateTable;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors de la suppression d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet qui correspond à la table supprimée</param>
        protected virtual void OnEventDeleteTable<T>(T obj)
        {
            Action<object> handler = EventDeleteTable;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors de la création d'une Base de Donnée
        /// </summary>
        /// <param name="name">Nom de la BDD</param>
        protected virtual void OnEventCreateDatabase(string name)
        {
            Action<string> handler = EventCreateDatabase;
            if (handler != null) handler(name);
        }

        /// <summary>
        ///     Event qui se déclenche lors d'un SELECT sur une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">ArborescenceFile d'objet obtenue</param>
        protected virtual void OnEventSelect<T>(T obj)
        {
            Action<object> handler = EventSelect;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors de la modification d'une ligne d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet qui a modifié la table</param>
        protected virtual void OnEventUpdate<T>(T obj)
        {
            Action<object> handler = EventUpdate;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors de la suppression d'une ligne d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet qui à servi à la suppression</param>
        protected virtual void OnEventDelete<T>(T obj)
        {
            Action<object> handler = EventDelete;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Event qui se déclenche lors d'un INSERT dans une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet qui a été ajouté</param>
        protected virtual void OnEvendAdd<T>(T obj)
        {
            Action<object> handler = EventAdd;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Initialisation des variables SQL
        /// </summary>
        public bool Initialize()
        {
            CheckConnexion = false;
            MgErreur = null;
            try
            {
                _connection = new MySqlConnection(CheckStringConnexion(false));

                if (OpenConnection())
                {
                    CheckConnexion = true;
                    CloseConnection();
                }
                else
                {
                    CheckConnexion = false;
                }
                return true;
            }
            catch (Exception exception)
            {
                MgErreur = exception.Message;
                return false;
            }
        }

        /// <summary>
        ///     Permet de changer la connexion courante
        /// </summary>
        /// <param name="createdBdd">Connexion à une BDD ou pas</param>
        /// <returns>string</returns>
        private string CheckStringConnexion(bool createdBdd)
        {
            try
            {
            //    return
            //        ConfigurationManager.ConnectionStrings["bdd"].ConnectionString;
                return createdBdd
                    ? "Server=" + Host + ";Uid=" + Pseudo + ";Pwd=" + Password + ";"
                    : "Server=" + Host + ";Database=" + Bdd + ";Uid=" + Pseudo + ";Pwd=" + Password + ";";
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Ouverture de la Connexion à la base de donnée
        /// </summary>
        /// <returns>TRUE si la connexion a réussie, FALSE sinon</returns>
        private bool OpenConnection()
        {
            try
            {
                if (ConnectionIsOpen()) return true;
                _connection.Open();
                return ConnectionIsOpen();
            }
            catch (MySqlException ex)
            {
                MgErreur = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// PErmet de vérifier si la connexion est ouverte
        /// </summary>
        /// <returns>bool</returns>
        private bool ConnectionIsOpen()
        {
            try
            {
                if (_connection.IsNull()) return false;
                return _connection.State == System.Data.ConnectionState.Open;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Fermeture de la connexion
        /// </summary>
        /// <returns>TRUE si la connexion est bien fermé, FALSE sinon</returns>
        public bool CloseConnection()
        {
            try
            {
                _connection.Close();
                if (_event)
                    OnEventCloseConnexion(null);
                return true;
            }
            catch (MySqlException ex)
            {
                MgErreur = ex.Message;
                return false;
            }
        }

        /// <summary>
        ///     Permet de fermer le MySqlDataReader
        /// </summary>
        /// <returns>bool</returns>
        public bool CloseDataReader()
        {
            try
            {
                if (_dataReader.IsClosed) return true;
                _dataReader.Dispose();
                _dataReader.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// PErmet de débuter une transaction
        /// </summary>
        /// <returns>bool</returns>
        public bool BeginTransaction()
        {
            try
            {
                if (OpenConnection().IsFalse()) return false;
                _transaction = _connection.BeginTransaction();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'effectuer les changements sur la bdd
        /// </summary>
        /// <returns>bool</returns>
        public bool CommitTransaction()
        {
            try
            {
                if (_transaction.IsNull() || OpenConnection().IsFalse()) return false;
                _transaction.Commit();
                _transaction.Dispose();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'annuler les changements sur la bdd
        /// </summary>
        /// <returns>bool</returns>
        public bool ErrorTransaction()
        {
            try
            {
                if (_transaction.IsNull() || OpenConnection().IsFalse()) return false;
                _transaction.Rollback();
                _transaction.Dispose();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de vérifier si il faut se reconnecter à une BDD
        /// </summary>
        /// <param name="bdd">Nom de la BDD</param>
        /// <param name="createBdd">Confirme la création d'une BDD</param>
        /// <returns>bool</returns>
        public bool VerifReconnexion(ref string bdd, bool createBdd = false)
        {
            try
            {
                //Si Bdd est null ou que ce c'est pas la même BDD, on déconnecte et on se connecte à la bonne BDD
                if (!Bdd.IsNullOrEmpty())
                    if (Bdd.Equals(bdd)) return true;

                if (!string.IsNullOrEmpty(bdd))
                    Bdd = bdd;
                if (string.IsNullOrEmpty(bdd))
                    bdd = Bdd;
                //On ferme la Co
                CloseConnection();
                _connection =
                    new MySqlConnection(CheckStringConnexion(createBdd));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de créer une BDD
        /// </summary>
        /// <param name="name">string</param>
        /// <returns>bool</returns>
        public bool CreateDatabase(string name)
        {
            try
            {
                //On regarde si la connexion est OK
                if (!OpenConnection()) return false;
                if (!VerifReconnexion(ref name)) return false;

                //Génération et controle de la liste
                if (!CheckBdd(name)) return false;

                _cmd = new MySqlCommand("CREATE DATABASE " + name, _connection);
                _cmd.ExecuteNonQuery();

                Bdd = name;

                if (_event)
                    OnEventCreateDatabase(Bdd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                CloseConnection();
            }
        }

        /// <summary>
        ///     Permet la création d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bdd">nom de la BDD</param>
        /// <param name="o">Object</param>
        /// <returns>bool</returns>
        public bool CreateTable<T>(T o, string bdd = null)
        {
            try
            {
                //Si Bdd est null ou que ce c'est pas la même BDD, on déconnecte et on se connecte à la bonne BDD
                if (!VerifReconnexion(ref bdd)) return false;

                if (!OpenConnection()) return false;

                //On récup les tables et on vérifie que la table n'existe pas
                if (!CheckTable(bdd, o.GetType().Name.ToLower()))
                {
                    CloseConnection();
                    return false;
                }

                //Génération de la requête
                string sql = GenerateSqlCreateTable(o.GetType().Name.ToLower(), ref o);
                if (sql.IsNullOrEmpty())
                {
                    CloseConnection();
                    return false;
                }

                //Execution de la requête
                _cmd = new MySqlCommand(sql, _connection);
                _cmd.ExecuteNonQuery();

                if (_event)
                    OnEventCreateTable(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                CloseConnection();
            }
        }

        /// <summary>
        /// Permet de créer et/ou update ue table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="bdd"></param>
        /// <returns>bool</returns>
        public bool CreateOrUpdateTable<T>(T o, string bdd = null)
        {
            try
            {
                CreateTable(o, bdd);
                UpdateTable(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de supprimer une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bdd">Nom de la BDD</param>
        /// <param name="o">Object</param>
        /// <returns>bool</returns>
        public bool DeleteTable<T>(string bdd, T o)
        {
            try
            {
                //Si Bdd est null ou que ce c'est pas la même BDD, on déconnecte et on se connecte à la bonne BDD
                if (!VerifReconnexion(ref bdd)) return false;

                if (!OpenConnection()) return false;

                //On récup les tables et on vérifie que la table existe
                if (CheckTable(bdd, o.GetType().Name.ToLower()))
                {
                    CloseConnection();
                    return false;
                }

                //Execution de la requête
                _cmd = new MySqlCommand("DROP TABLE " + o.GetType().Name.ToLower(), _connection);
                _cmd.ExecuteNonQuery();

                if (_event)
                    OnEventDeleteTable(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                CloseConnection();
            }
        }

        /// <summary>
        ///     Permet de modifier une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Object</param>
        /// <returns>bool</returns>
        public bool UpdateTable<T>(T o)
        {
            try
            {
                if (!OpenConnection()) return false;

                List<string> listeColum = GetChampTable(o.GetType().Name.ToLower());
                PropertyInfo[] listePropertieName = o.GetType().GetProperties();

                if (listeColum.Count <= 0 || listePropertieName.Equals(null)) return true;

                //On génére la List<string> des properties
                List<string> temp = o.GetNameProperties();

                List<string> listeColumTemp = listeColum.DeleteDoublonWiteOtherListEquals(temp);
                List<string> listePropertieNameTemp = temp.DeleteDoublonWiteOtherListEquals(listeColum);

                //On enléve les ID
                listeColumTemp = listeColumTemp.DeleteDoublonContains(new[] { "Id_", "id_" });
                listePropertieNameTemp = listePropertieNameTemp.DeleteDoublonEquals(new[] { "Id", "id" });

                if (listeColumTemp.Count > 0)
                {
                    //On doit supprimer des colonnes dans la table
                    foreach (string colonne in listeColumTemp)
                    {
                        _cmd = new MySqlCommand("ALTER TABLE " + o.GetType().Name.ToLower() + " DROP " + colonne,
                            _connection);
                        _cmd.ExecuteNonQuery();
                        _cmd.Dispose();
                    }
                }
                else if (listePropertieNameTemp.Count > 0)
                {
                    foreach (string pr in listePropertieNameTemp)
                    {
                        //On doit ajouter une colonne dans la table
                        string pr1 = pr;
                        foreach (string str in listePropertieName.Where(prop => prop.Name.Equals(pr1)).Select(GetTypeField).Where(str => !str.IsNullOrEmpty()))
                        {
                            _cmd = new MySqlCommand("ALTER TABLE " + o.GetType().Name.ToLower() + " ADD " + pr + " " + str, _connection);
                            _cmd.ExecuteNonQuery();
                            _cmd.Dispose();
                        }
                    }
                }

                if (_event)
                    OnEventUpdateTable(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                CloseConnection();
            }
        }

        /// <summary>
        ///     Permet d'ajouter un objet dans une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Objet</param>
        /// <param name="bdd">No de la BDD</param>
        /// <returns>bool</returns>
        public int Add<T>(T o, string bdd = null)
        {
            try
            {
                //Verif BDD
                if (string.IsNullOrEmpty(bdd) && !string.IsNullOrEmpty(Bdd)) bdd = Bdd;
                if (string.IsNullOrEmpty(bdd)) return -1;

                //Verif
                if (!VerifReconnexion(ref bdd)) return -1;
                if (!OpenConnection()) return -1;

                //On récup la liste des champs de la table sans l'id auto_increment
                List<string> listeChamp = GetChampTable(o.GetType().Name.ToLower(),
                    "id_" + o.GetType().Name.ToLower());

                //Création de la requête
                string sql = "INSERT INTO " + o.GetType().Name.ToLower() + "(" + listeChamp.Join(",") +
                             ") VALUES (";
                //On crée les variables préparés pour la requête
                for (int i = 0; i < listeChamp.Count; i++)
                {
                    if (i == (listeChamp.Count - 1))
                        sql += "@" + listeChamp[i] + ")";
                    else
                        sql += "@" + listeChamp[i] + ",";
                }

                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if(_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                //On assigne les valeurs des variables préparées
                if (!AttribuerValeurRequetePreparees(ref o)) return -1;

                //Execute command
                _cmd.ExecuteNonQuery();
                var lastId = (int)_cmd.LastInsertedId;

                //Si les events ne sont pas permis, on return l'id
                if (!_event) return lastId;

                //On ajoute l'id à l'objet
                PropertyInfo propertieId = o.GetType().GetProperty("Id");
                if (propertieId != null)
                {
                    propertieId.SetValue(o, lastId, null);
                }
                //On appelle l'event
                OnEvendAdd(o);
                return lastId;
            }
            catch (Exception)
            {
                return -2;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if(_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        public int AddQuery(string sql = null)
        {
            try
            {
                //Verif BDD
                if (sql.IsNullOrEmpty()) return -1;

                //Verif
                if (!OpenConnection()) return -1;

                //Création de la commande
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                //Execute command
                _cmd.ExecuteNonQuery();
                var lastId = (int)_cmd.LastInsertedId;

                return lastId;
            }
            catch (Exception)
            {
                return -2;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        ///     Permet de supprimer une entrée via son id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Object</param>
        /// <param name="id">id de l'entrée</param>
        /// <param name="bdd">Nom de la BDD</param>
        /// <returns>bool</returns>
        public bool DeleteById<T>(T o, int id = 0, string bdd = null)
        {
            try
            {
                if (id == 0) return false;

                //Verif BDD
                if (string.IsNullOrEmpty(bdd) && !string.IsNullOrEmpty(Bdd)) bdd = Bdd;
                if (string.IsNullOrEmpty(bdd)) return false;

                //Verif
                if (!VerifReconnexion(ref bdd)) return false;
                if (!OpenConnection()) return false;

                //Construction de la requête
                string sql = "DELETE FROM " + o.GetType().Name.ToLower() + " WHERE id_" + o.GetType().Name.ToLower() +
                             " = " + id;
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                _cmd.ExecuteNonQuery();
                if (_event)
                    OnEventDelete(o);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        ///     Permet de supprimer une entrée via son id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Object</param>
        /// <param name="tabId"></param>
        /// <param name="bdd">Nom de la BDD</param>
        /// <returns>bool</returns>
        public bool DeleteByMultiId<T>(T o, int[] tabId = null, string bdd = null)
        {
            try
            {
                if (tabId.NullOrEmpty()) return false;

                //Verif BDD
                if (string.IsNullOrEmpty(bdd) && !string.IsNullOrEmpty(Bdd)) bdd = Bdd;
                if (string.IsNullOrEmpty(bdd)) return false;

                //Verif
                if (!VerifReconnexion(ref bdd)) return false;
                if (!OpenConnection()) return false;

                //Construction de la requête
                string sql = "DELETE FROM " + o.GetType().Name.ToLower() + " WHERE id_" + o.GetType().Name.ToLower() +
                             " IN (" + (tabId.Join(","))+")";
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                _cmd.ExecuteNonQuery();
                if (_event)
                    OnEventDelete(o);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Permet de compter le nombre d'entrées d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public int Count<T>(T o)
        {
            try
            {
                if (o.IsNull()) return -1;
                if (!OpenConnection()) return -1;

                //Construction de la requête
                string sql = "SELECT count(*) FROM " + o.GetType().Name.ToLower();
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                return Convert.ToInt32(_cmd.ExecuteScalar());
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        ///     Permet de modifier une entrée via l'id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Object</param>
        /// <param name="id">Id de l'entrée</param>
        /// <param name="bdd">Nom de la Bdd par défault</param>
        /// <returns>bool</returns>
        public bool UpdateById<T>(T o, int id = 0, string bdd = null)
        {
            try
            {
                if (id == 0) return false;

                //Verif BDD
                if (string.IsNullOrEmpty(bdd) && !string.IsNullOrEmpty(Bdd)) bdd = Bdd;
                if (string.IsNullOrEmpty(bdd)) return false;

                //Verif
                if (!VerifReconnexion(ref bdd)) return false;
                if (!OpenConnection()) return false;

                //On supprime les variable à ignorer
                var listeProperties = o.GetType().GetProperties().Where(propertyInfo => !propertyInfo.Name.Equals("Id")).ToList();
                var listePropertiesIgnore = new List<PropertyInfo>();
                foreach (
                    PropertyInfo propertyInfo in
                        listeProperties)
                {
                    var propertieType = GetTypeField(propertyInfo);
                    if (!propertieType.IsNullOrEmpty()) continue;
                    listePropertiesIgnore.Add(propertyInfo);
                }

                if (listePropertiesIgnore.Any())
                {
                    foreach (var propertyInfo in listePropertiesIgnore)
                    {
                        listeProperties.RemoveElement(propertyInfo);
                    }
                }
                //Début de la requête
                string sql = "UPDATE " + o.GetType().Name.ToLower() + " SET ";
                //Création des variables préparées
                foreach (
                    PropertyInfo propertyInfo in
                        listeProperties)
                {
                    
                    if (listeProperties.CheckLastElement(propertyInfo))
                        sql += propertyInfo.Name + " = @" + propertyInfo.Name;
                    else
                        sql += propertyInfo.Name + " = @" + propertyInfo.Name + ", ";
                }
                //Fin requête
                sql += " WHERE id_" + o.GetType().Name.ToLower() + " = " + id;

                //Création de la commande
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                //Attribution des valeurs aux variables préparées
                if (!AttribuerValeurRequetePreparees(ref o)) return false;

                //Execute Command
                _cmd.ExecuteNonQuery();

                if (_event)
                    OnEventUpdate(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Permet d'executer une requête manuellement
        /// </summary>
        /// <param name="sql">La requête</param>
        /// <returns>bool</returns>
        public bool ManualUpdate(string sql)
        {
            try
            {
                //Verif BDD
                if (sql.IsNullOrEmpty()) return false;

                //Verif
                if (!OpenConnection()) return false;

                //Création de la commande
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                //Execute Command
                return _cmd.ExecuteNonQuery() != 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Permet d'executer une requête manuellement
        /// </summary>
        /// <param name="sql">La requête</param>
        /// <returns>bool</returns>
        public bool ManualDelete(string sql)
        {
            try
            {
                //Verif BDD
                if (sql.IsNullOrEmpty()) return false;

                //Verif
                if (!OpenConnection()) return false;

                //Création de la commande
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                //Execute Command
                return _cmd.ExecuteNonQuery() != 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Permet d'executer une requête select manuellement
        /// </summary>
        /// <param name="sql">La requête</param>
        /// <param name="o">Objet de type de référence à retourner</param>
        /// <returns>bool</returns>
        public List<T> ManualSelect<T>(string sql, T o) where T : new()
        {
            try
            {
                //Verif BDD
                if (sql.IsNullOrEmpty()) return new List<T>();

                //Verif
                if (!OpenConnection()) return new List<T>();

                //Création de la commande
                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                _dataReader = _cmd.ExecuteReader();

                PropertyInfo[] listeProperties = o.GetPropertyInfo();
                var listeResult = new List<T>();

                while (_dataReader.Read())
                {
                    listeResult.Add(DataReaderToT(_dataReader, listeProperties, new T()));
                }

                return listeResult;
            }
            catch (Exception)
            {
                return new List<T>();
            }
            finally
            {
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        ///     Permet de récupèrer des entrées d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Object</param>
        /// <param name="condition">tableau d'objet des conditions</param>
        /// <param name="bdd">BDD à laquelle se connecter, default null</param>
        /// <returns></returns>
        public List<T> Select<T>(T o, RField[] condition = null, string bdd = null) where T : new()
        {
            try
            {
                //Verif BDD
                if (string.IsNullOrEmpty(bdd) && !string.IsNullOrEmpty(Bdd)) bdd = Bdd;
                if (string.IsNullOrEmpty(bdd)) return new List<T>();

                //Verif
                if (!VerifReconnexion(ref bdd)) return new List<T>();
                if (!OpenConnection()) return new List<T>();

                //Verif qu'au moins un des operator de RField soit null
                var c = new List<RField>();
                if (condition != null && condition.Length > 0)
                {
                    RField fieldTemp = null;
                    foreach (RField field in condition.Where(field => field.ConditionOperator == null))
                    {
                        fieldTemp = field;
                    }
                    if (fieldTemp == null) return new List<T>();

                    //On fait passer le field avec operator null en premier dans la liste
                    c = condition.ToList();
                    c.MoveElement(fieldTemp);
                }

                //On vérifie l'ORBER BY
                var listeTemp = c.Where(rField => rField.Operator.Contains("ORDER") || rField.Operator.Contains("order")).ToList();

                if (listeTemp.Any())
                {
                    foreach (var rField in listeTemp)
                    {
                        c.Remove(rField);
                    }
                    c.AddRange(listeTemp);
                }
                listeTemp.Clear();

                //On vérifie la LIMIT
                listeTemp = c.Where(rField => rField.Operator.Contains("LIMIT") || rField.Operator.Contains("limit")).ToList();
                if (listeTemp.Any())
                {
                    foreach (var rField in listeTemp)
                    {
                        c.Remove(rField);
                    }
                    c.AddRange(listeTemp);
                }
                listeTemp.Clear();

                //On récup les nom des colonnes de la table
                PropertyInfo[] listeProperties = o.GetPropertyInfo();
                string sql;
                if (condition != null && condition.Length > 0)
                {
                    sql = "SELECT * FROM " + o.GetType().Name.ToLower();
                    //Check only ORDER or LIMIT
                    var check = false;
                    foreach (var field in c)
                    {
                        if (!field.Operator.Contains("order") && !field.Operator.Contains("ORDER") && !field.Operator.Contains("limit") && !field.Operator.Contains("LIMIT"))
                        {
                            check = true;
                        }
                    }
                    //On regarde si on fait qu'un ORDER BY
                    if (check)
                    {
                        sql += " WHERE ";
                    }
                    foreach (RField field in c)
                    {
                        if (field.Operator.Contains("LIKE"))
                        {
                            sql += field.ConditionOperator + " " + field.Name + " LIKE " + "@" + field.Name;
                        }
                        else if (field.Operator.Contains("ORDER") || field.Operator.Contains("order"))
                        {
                            sql = sql + (" " + field.Operator + " " + field.Name + " " + field.Value + " ");
                        }
                        else if (field.Operator.Contains("LIMIT") || field.Operator.Contains("limit"))
                        {
                            var v = field.Value.ToString().Split(',');
                            sql = sql + (" " + field.Operator + " " + v[0] + ", " + v[1] + " ");
                        }
                        else if (field.Operator.Contains("IN") || field.Operator.Contains("in"))
                        {
                            sql = sql + (" " + field.Name + " " + field.Operator + " (" + field.Value + ") ");
                        }
                        else if (field.Operator.Contains("NOT IN") || field.Operator.Contains("not in"))
                        {
                            sql = sql + (" " + field.Name + " " + field.Operator + " (" + field.Value + ") ");
                        }
                        else if (field.Operator.Contains("IS") || field.Operator.Contains("is"))
                        {
                            sql = sql + (" " + field.Name + " " + field.Operator + " " + field.Value + " ");
                        }
                        else
                        {
                            sql = sql +
                                  (field.ConditionOperator + " " + field.Name + " " + field.Operator + " @" + field.Name +
                                   " ");
                        }
                    }
                }
                else
                    sql = "SELECT * FROM " + o.GetType().Name.ToLower();

                _cmd = new MySqlCommand(sql, _connection);

                //On affecte la transaction si elle existe
                if (_transaction.IsNotNull())
                {
                    _cmd.Transaction = _transaction;
                }

                //On attribut les valeur aux variables préparés
                var x = c.ToArray();
                if (!AttribuerValeurRequetePreparees(ref x)) return new List<T>();

                _dataReader = _cmd.ExecuteReader();

                var listeResult = new List<T>();

                while (_dataReader.Read())
                {
                    listeResult.Add(DataReaderToT(_dataReader, listeProperties, new T()));
                }

                if (_event)
                    OnEventSelect(listeResult);

                return listeResult;
            }
            catch (Exception)
            {
                return new List<T>();
            }
            finally
            {
                if (_dataReader != null)
                    CloseDataReader();
                if (_cmd != null)
                    _cmd.Dispose();
                if (_transaction.IsNull())
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        ///     Permet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <param name="property"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private static T DataReaderToT<T>(MySqlDataReader dataReader, IEnumerable<PropertyInfo> property, T o)
        {
            try
            {
                foreach (PropertyInfo v in property)
                {
                    if (v.NameProperty().Equals("System.String"))
                    {
                        o.GetPropertieByName(v.Name)
                            .SetValue(o, dataReader.GetValue<string>(v.Name, "string"), null);
                    }
                    else if (v.NameProperty().Equals("System.Int32"))
                    {
                        o.GetPropertieByName(v.Name).SetValue(o,
                            v.Name.Equals("Id")
                                ? dataReader.GetValue<int>("id_" + o.GetType().Name.ToLower(), "int")
                                : dataReader.GetValue<int>(v.Name, "int"), null);
                    }
                    else if (v.NameProperty().Equals("System.Double"))
                    {
                        o.GetPropertieByName(v.Name)
                            .SetValue(o, dataReader.GetValue<double>(v.Name, "double"), null);
                    }
                    else if (v.NameProperty().Equals("System.Single"))
                    {
                        o.GetPropertieByName(v.Name).SetValue(o, dataReader.GetValue<float>(v.Name, "float"), null);
                    }
                    else if (v.NameProperty().Equals("System.DateTime"))
                    {
                        o.GetPropertieByName(v.Name).SetValue(o, dataReader.GetValue<DateTime>(v.Name, "DateTime"), null);
                    }
                }
                return o;
            }
            catch (Exception)
            {
                return o;
            }
        }

        /// <summary>
        ///     Permet de récupèrer la liste des BDD
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckBdd(string dataBase)
        {
            try
            {
                CloseConnection();
                _connection =
                    new MySqlConnection(CheckStringConnexion(true));
                OpenConnection();
                _cmd = new MySqlCommand("SHOW DATABASES", _connection);
                _dataReader = _cmd.ExecuteReader();

                var listeBdd = new List<string>();

                while (_dataReader.Read())
                {
                    listeBdd.Add(_dataReader.GetString("Database"));
                }

                return listeBdd.All(bdd => !bdd.Equals(dataBase));
            }
            catch (Exception)
            {
                return true;
            }
            finally
            {
                if (_dataReader != null)
                    _dataReader.Dispose();
                if (_cmd != null)
                    _cmd.Dispose();
            }
        }

        /// <summary>
        ///     Permet de vérifier si une table existe dans une BDD
        /// </summary>
        /// <param name="bdd">string</param>
        /// <param name="table">La table</param>
        /// <returns>bool</returns>
        private bool CheckTable(string bdd, string table)
        {
            try
            {
                _cmd = new MySqlCommand("SHOW TABLES", _connection);
                _dataReader = _cmd.ExecuteReader();

                var listeTable = new List<string>();

                while (_dataReader.Read())
                {
                    listeTable.Add(_dataReader.GetString("Tables_in_" + bdd));
                }

                return listeTable.All(t => !t.Equals(table));
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_dataReader != null)
                    CloseDataReader();
                if (_cmd != null)
                    _cmd.Dispose();
            }
        }

        /// <summary>
        ///     On récupère touts les champs de la table
        /// </summary>
        /// <param name="table">Nom de la table sur laquelle nous voulons obtenir les champs</param>
        /// <param name="colonne">Champ à ne pas récupérer</param>
        /// <returns>List contenant les champs de la table</returns>
        private List<string> GetChampTable(string table, string colonne = "id")
        {
            try
            {
                var listeTable = new List<string>();

                _cmd = new MySqlCommand("SHOW COLUMNS FROM " + table, _connection);
                //Create a data reader and Execute the command
                _dataReader = _cmd.ExecuteReader();

                while (_dataReader.Read())
                {
                    if (!_dataReader.GetString("Field").Equals(colonne))
                        listeTable.Add(_dataReader.GetString("Field"));
                }
                return listeTable;
            }
            catch (Exception)
            {
                return new List<string>();
            }
            finally
            {
                if (_dataReader != null)
                    CloseDataReader();
                if (_cmd != null)
                    _cmd.Dispose();
            }
        }

        /// <summary>
        ///     Permet de générer la requête SQL pour la création d'une table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameTable">Nom de la table</param>
        /// <param name="o">Object</param>
        /// <returns>string</returns>
        private string GenerateSqlCreateTable<T>(string nameTable, ref T o)
        {
            try
            {
                if (o.Equals(null)) return null;

                //On récup les property de l'objet

                string sql = "CREATE TABLE " + nameTable + "( id_" + nameTable.ToLower() +
                             " int(11) NOT NULL AUTO_INCREMENT, ";

                //On récup les properties de l'objet
                foreach (
                    PropertyInfo propertyInfo in
                        o.GetType().GetProperties().Where(propertyInfo => propertyInfo.Name != "Id"))
                {

                    var type = GetTypeField(propertyInfo);

                    if (type == null) continue;

                    sql += propertyInfo.Name + " " + type + ",";
                }

                sql += "PRIMARY KEY (`id_" + nameTable.ToLower() +
                       "`) )  ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1;";
                return sql;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Récupère le type SQL d'une variable
        /// </summary>
        /// <param name="propertie"></param>
        /// <returns>string</returns>
        private string GetTypeField(PropertyInfo propertie)
        {
            try
            {
                if (propertie.Equals(null)) return null;

                //on récup les attributs de la PropertyInfo
                Dictionary<string, object> listeAttribut = propertie.GetAttribut();
                foreach (var o in listeAttribut)
                {
                    switch (o.Key)
                    {
                        case "RsqlVarchar":
                            return "varchar(" + o.Value + ") DEFAULT NULL";
                        case "RsqlInt":
                            return "int(11) NOT NULL";
                        case "RsqlText":
                            return "text";
                        case "RsqlDouble":
                            return "double DEFAULT NULL";
                        case "RsqlFloat":
                            return "float DEFAULT NULL";
                        case "RsqlDateTime":
                            return "datetime NOT NULL";
                        default:
                            return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        ///     Permet d'attribuer des valeurs aux variables des requêtes préparées
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">Object</param>
        /// <returns></returns>
        private bool AttribuerValeurRequetePreparees<T>(ref T o)
        {
            try
            {
                foreach (
                    PropertyInfo propertyInfo in
                        o.GetType().GetProperties().Where(propertyInfo => !propertyInfo.Name.Equals("Id")))
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + propertyInfo.Name, MySqlDbType.Text));
                        if (propertyInfo.GetValue(o, null) == null ||
                            propertyInfo.GetValue(o, null).ToString().IsNullOrEmpty())
                            _cmd.Parameters["@" + propertyInfo.Name].Value = null;
                        else
                            _cmd.Parameters["@" + propertyInfo.Name].Value =
                                propertyInfo.GetValue(o, null).ToString().Trim();
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + propertyInfo.Name, MySqlDbType.Int32));
                        _cmd.Parameters["@" + propertyInfo.Name].Value = propertyInfo.GetValue(o, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(double))
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + propertyInfo.Name, MySqlDbType.Double));
                        _cmd.Parameters["@" + propertyInfo.Name].Value = propertyInfo.GetValue(o, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(float))
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + propertyInfo.Name, MySqlDbType.Float));
                        _cmd.Parameters["@" + propertyInfo.Name].Value = propertyInfo.GetValue(o, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + propertyInfo.Name, MySqlDbType.DateTime));
                        _cmd.Parameters["@" + propertyInfo.Name].Value = propertyInfo.GetValue(o, null);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool AttribuerValeurRequetePreparees(ref RField[] tab)
        {
            try
            {
                foreach (RField field in tab)
                {
                    if (field.Value is string)
                    {
                        //Si il contient un ORDER, on le saute
                        if (field.Operator.Contains("ORDER")) continue;

                        string value;
                        _cmd.Parameters.Add(new MySqlParameter("@" + field.Name, MySqlDbType.Text));
                        if (field.Operator.Equals("%LIKE%"))
                        {
                            value = "%" + field.Value + "%";
                        }
                        else if (field.Operator.Equals("%LIKE"))
                        {
                            value = "%" + field.Value;
                        }
                        else if (field.Operator.Equals("LIKE%"))
                        {
                            value = field.Value + "%";
                        }
                        else
                        {
                            value = field.Value.ToString();
                        }
                        _cmd.Parameters["@" + field.Name].Value =
                            string.IsNullOrEmpty(value)
                                ? null
                                : value.Trim();
                    }
                    else if (field.Value is int)
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + field.Name, MySqlDbType.Int32));
                        _cmd.Parameters["@" + field.Name].Value = field.Value;
                    }
                    else if (field.Value is double)
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + field.Name, MySqlDbType.Double));
                        _cmd.Parameters["@" + field.Name].Value = field.Value;
                    }
                    else if (field.Value is float)
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + field.Name, MySqlDbType.Float));
                        _cmd.Parameters["@" + field.Name].Value = field.Value;
                    }
                    else if (field.Value is DateTime)
                    {
                        _cmd.Parameters.Add(new MySqlParameter("@" + field.Name, MySqlDbType.DateTime));
                        _cmd.Parameters["@" + field.Name].Value = field.Value;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}