# SqlCRUDRepository

SqlCRUDRepository is a .net library To perform read, insert, edit and delete operations on SQL Server database tables.

This project was developed for Microsoft SQL Server. You can extend this project for other databases.

-----
# Samples

## 1. How to create a new record in a table?
Suppose you have a table called tblCustomer in a database.

Create Table tblCustomer (ID int Identity (1,1), FirstName nvarchar (max), LastName nvarchar (max))

we create a model from this table.(In later sections we will change the class name so that it is not the same as the table name)

    public class tblCustomer
    {
        public Int32 ID {get; set; }
        public string FirstName {get; set; }
        public string LastName {get; set; }
    }
    
then save it!


    var Sampledb = Repository.RepositoryFactory.CreateRepository (ConnectionString);
    tblCustomer customer = new tblCustomer ();
    customer.FirstName = "FirstNameCustomer";
    customer.LastName = "LastNameCustomer";
    Sampledb.Save (customer);
    Console.WriteLine (customer.ID);

* You can use IDbConnection instead of ConnectionString for added security.
* After saving in the table, an ID will be given to the customer.
## 2. How to find a customer with an ID and change its fields?
            var customerdb = Sampledb.GetByID <tblCustomer> (customer.ID.ToString (), false);
            customerdb.FirstName = "ChangedFirstNameCustomer";
            Sampledb.Save (customerdb);
* Note: properties with null value ignored in SQl update statment .

    for example.

            tblCustomer customer = new tblCustomer ();
            customer.ID=3;
            customer.FirstName = "ChangedFirstNameCustomer";
            Sampledb.Save (customer);
            
    Update tblCustomer without change LastName field in database. (LastName is nullable and has value null in sample code)           
    
## 3. How to remove customer from database?
Sampledb.Delete (customerdb); // you can also use DeleteList(batch delete) In latest version.
## 4. How do I use transactions?
            Sampledb.BeginTransaction ();
            tblCustomer customer = new tblCustomer ();
            customer.FirstName = "FirstNameCustomer";
            customer.LastName = "LastNameCustomer";
            Sampledb.Save (customer);
            var customerdb = Sampledb.GetByID <tblCustomer> (customer.ID.ToString (), false);
            customerdb.FirstName = "ChangedFirstNameCustomer";
            Sampledb.Save (customerdb);
            Sampledb.Delete (customerdb);
            Sampledb.CommitTransaction ();
## 5. How do I change the model name?
    [Repository.Domain.TableInfo ("tblCustomer", "ID", true)]
    public class Customer
    {
        public Int32 ID {get; set; }
        public string FirstName {get; set; }
        public string LastName {get; set; }
    }
* In this section you can create a read-only or write-only table.
## 6. Can the name of the class property be different from the name of the fields in my table?
Yes. Using [Repository.Domain.FieldInfo ("FirstName", false)]

    [Repository.Domain.TableInfo ("tblCustomer", "ID", true)]
    public class Customer
    {
        public Int32 ID {get; set; }
        [Repository.Domain.FieldInfo ("FirstName", false)]
        public string FirstName {get; set; }
        public string LastName {get; set; }
    }

## 7. What can be done for properties that exist in the class but do not exist in the database?
        [Repository.Domain.FieldInfo ("CustomProperty", true)]
        public int CustomProperty {get; set; }
## 8. How do I call a StoredProcedure?
var result = Sampledb.Execute_StoredProcedure <Customer> (spName, null);
* Up to 10 ResultSet can be obtained with one call.
## 9. How can I find specific values ​​in a table? For example, all customers whose name is equal to a certain value?
            string Filter = "FirstName = @ FirstName";
            Dictionary <string, string> parameters = new Dictionary <string, string> ();
            parameters.Add ("FirstName", "FirstNameCustomer");
            var result = Sampledb.Find <Customer> (Filter, "ID", false, parameters, "");
            Console.WriteLine (result.Count);

* In this section you can both sort and name the fields you just want so that only those fields are provided for you.
    
* In the Find and GetByID methods, a parameter called withLock is considered, which can also be used to lock table records when reading. In cases where there is a transaction, this can be useful to make the synchronization controllable.
    
## 10. Can we give the ID generation mechanism ourselves, for example not the Identity table?
Yes.
To do this, you must first change the model(Both database and code).
    
    Create Table tblCustomer2(ID nvarchar(500), FirstName nvarchar(max), CustomerLastName nvarchar(max),Description nvarchar(max))
    
    [Repository.Domain.TableInfo("tblCustomer2","ID",false)]
    public class Customer
    {
        public string ID { get; set; }
        [Repository.Domain.FieldInfo("FirstName",false)]
        public string FirstName { get; set; }
        [Repository.Domain.FieldInfo("CustomerLastName", false)]
        public string LastName { get; set; }

        [Repository.Domain.FieldInfo("CustomProperty", true)]
        public int CustomProperty { get; set; }
    }

There are two mechanisms for creating an ID
1. In the program code (this model is available in version 1.0.14 and above)
    
    You must have a function to create an ID.
    
    For example
    
        private string keygenerator(string ClassName)
        {
            return Guid.NewGuid().ToString();
        }
    
    And at the end
    
    Sampledb.__ KeyGenerator = keygenerator;
    
    
2. In the database (default)
    
    There is a stored procedure in the database called __KeyGenerator that must be overwritten.
    
## 11. Is it possible to update in batches?
Yes.

For example
This command updates the first name column of all records whose ID is not 2.

       Customer o = new Customer();
       o.FirstName = "a";
       Sampledb.Save(o,"ID <> 2");

## 12. How can I find the first record matching the filter?
            string Filter = "FirstName = @FirstName";
            Dictionary <string, string> parameters = new Dictionary <string, string> ();
            parameters.Add ("FirstName", "FirstNameCustomer");
            var result = Sampledb.FindFirst <Customer> (Filter, "ID", false, parameters, "");

## 13. Is it possible to insert in a batch?
Yes.
For example
            List<Customer> lst = new List<Customer>();
            for (int i = 0; i < 10000; i++)
            {
                Customer o2 = new Customer();
                o2.FirstName = body.FirstName;
                o2.LastName = body.LastName;
                lst.Add(o2);
            }
            Sampledb.SaveList(lst, "ID");
            return "1";    
## License
[MIT](https://licenses.nuget.org/MIT)