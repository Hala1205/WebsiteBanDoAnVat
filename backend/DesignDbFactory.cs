using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
public class DesignDbFactory : IDesignTimeDbContextFactory<Db>
{
 public Db CreateDbContext(string[] args)
 {
  var cs=Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? throw new Exception("Set ConnectionStrings__Default for migrations");
  return new Db(new DbContextOptionsBuilder<Db>().UseSqlServer(cs).Options);
 }
}
