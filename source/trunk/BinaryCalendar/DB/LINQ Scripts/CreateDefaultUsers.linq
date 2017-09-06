<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>BinaryCalendar</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

Users.DeleteAllOnSubmit(Users);
SubmitChanges();

User admin = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "paulk",
	Password = "27830529",
	EmailAddress = "paul.kolozsvari@binarychef.com",
	Role = 7,
	Notes = "Admin user",
	DateCreated = DateTime.Now
};
User standardPharmacy = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "StandardPharmacy",
	Password = "$t@nd@Rd",
	EmailAddress = "standardpharmacy@telkomsa.net",
	Role = 2,
	Notes = "Admin user",
	DateCreated = DateTime.Now
};
Users.InsertOnSubmit(admin);
Users.InsertOnSubmit(standardPharmacy);
SubmitChanges();

Users.Dump();