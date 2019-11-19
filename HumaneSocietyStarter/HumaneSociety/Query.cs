using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch(crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    employee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    if(employee == null)
                    {
                        throw new Exception("Null record was found.");
                    }
                    break;
                case "update":
                    Employee employeeFromDatabase = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    employeeFromDatabase.FirstName = employee.FirstName;
                    employeeFromDatabase.LastName = employee.LastName;
                    employeeFromDatabase.EmployeeNumber = employee.EmployeeNumber;
                    employeeFromDatabase.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    return;
            }
        }

       

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            // CREATE
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            // READ
            Animal animal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            if (animal == null)
            {
                throw new Exception("No animal found.");
            }
            return(animal);
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            // UPDATE
            string input;
            Animal animalfromDatabase = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            if (updates.TryGetValue(1, out input))
            {
                int inputInt;
                if (int.TryParse(input, out inputInt))
                {
                    animalfromDatabase.CategoryId = inputInt;
                }
            }
            if (updates.TryGetValue(2, out input))
            {
                animalfromDatabase.Name = input;
            }
            
            if (updates.TryGetValue(3, out input))
            {
                int inputInt;
                if (int.TryParse(input, out inputInt))
                {
                    animalfromDatabase.Age = inputInt;
                }
            }
            if (updates.TryGetValue(4, out input))
            {
                animalfromDatabase.Demeanor = input;
            }
            if (updates.TryGetValue(5, out input))
            {
                if (input == "false")
                {
                    animalfromDatabase.KidFriendly = false;
                }
                else
                {
                    animalfromDatabase.KidFriendly = true;
                }
            }
            if (updates.TryGetValue(6, out input))
            {
                if (input == "false")
                {
                    animalfromDatabase.PetFriendly = false;
                }
                else
                {
                    animalfromDatabase.PetFriendly = true;
                }
            }
            if (updates.TryGetValue(7, out input))
            {
                int inputInt;
                if (int.TryParse(input, out inputInt))
                {
                    animalfromDatabase.Weight = inputInt;
                }
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            // REMOVE
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(int animalId, Dictionary<int, string> updates) // parameter(s)?
        {


            List<Animal> animals = db.Animals.ToList();

            foreach(KeyValuePair<int, string> pair in updates)
            {
                switch (pair.Key)
                {
                    case 1:
                        //need to fix this shit
                        animals = animals.Where(a => a.CategoryId == db.Categories.Where(c => c.Name == pair.Value).Select(c => c.CategoryId)).ToList();
                        break;
                    case 2:
                        animals = animals.Where(a => a.Name == pair.Value).ToList();
                        break;
                    case 3:
                        animals = animals.Where(a => a.Age == Convert.ToInt32(pair.Value)).ToList();
                        break;
                    case 4:
                        animals = animals.Where(a => a.Demeanor == pair.Value).ToList();
                        break;
                    case 5:
                        if (pair.Value == "true")
                        {
                            animals = animals.Where(a => a.KidFriendly == Convert.ToBoolean(true)).ToList();
                        }
                        break;
                    case 6:
                        if (pair.Value == "true")
                        {
                            animals = animals.Where(a => a.PetFriendly == Convert.ToBoolean(true)).ToList();
                        }
                        break;
                    case 7:
                        animals = animals.Where(a => a.Weight == Convert.ToInt32(pair.Value)).ToList();
                        break;
                }
            }
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            throw new NotImplementedException();
        }
        
        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}