using System;

namespace ApiEcommerce.Repository.IRepository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool CategoryExists(int id)
    {
        return _db.Categories.Any(c => c.Id == id);
    }

    public bool CategoryExists(string name)
    {
        // tolowercase and trim the name to avoid case sensitivity and whitespace issues
        return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _db.Categories.Add(category);
        return Save();
    }

    public bool DeleteCategory(Category category)
    {
        _db.Categories.Remove(category);
        return Save();
    }

    public ICollection<Category> GetCategories()
    {
        // return _db.Categories.OrderBy(c => c.Name).ToList();
        return _db.Categories.OrderBy(c => c.Id).ToList();
    }

    public Category? GetCategory(int id)
    {
        return _db.Categories.FirstOrDefault(c => c.Id == id);
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0 ? true : false;
    }

    public bool UpdateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _db.Categories.Update(category);
        return Save();
    }
}


// NOTE:
// el patron repository es una capa de abstracción que se utiliza para separar la lógica de acceso a datos de la lógica de negocio. Esto permite que la lógica de negocio no dependa directamente de la forma en que se almacenan y recuperan los datos, lo que facilita el mantenimiento y la prueba del código. En este caso, el CategoryRepository implementa la interfaz ICategoryRepository, lo que permite que la lógica de negocio interactúe con los datos de las categorías sin preocuparse por cómo se almacenan o recuperan.