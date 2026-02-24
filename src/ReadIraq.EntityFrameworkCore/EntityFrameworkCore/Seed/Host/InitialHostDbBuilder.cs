namespace ReadIraq.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly ReadIraqDbContext _context;

        public InitialHostDbBuilder(ReadIraqDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            //new InitialSampleDataBuilder(_context).Create();

            _context.SaveChanges();
        }
    }
}
