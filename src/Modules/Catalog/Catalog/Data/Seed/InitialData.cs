namespace Catalog.Data.Seed;

public static class InitialData
{
    public static IEnumerable<Product> Products =>
        new List<Product>
        {
            Product.Create(new Guid("9121DDC5-9F89-491C-8C80-19F2AE39281D"),"IPhone X",["Category1"],"Long descriptions","imagefile",500),
            Product.Create(new Guid("8139715D-94F8-4B41-8D64-160BBEDA91D6"),"Samsung 10",["Category1"],"Long descriptions","imagefile",400),
            Product.Create(new Guid("7139715D-9F89-491C-8C80-19F2AE39281D"),"Huawei Plus",["Category2"],"Long descriptions","imagefile",650),
            Product.Create(new Guid("6139715D-9F89-491C-8C80-19F2AE39281D"),"Xiaomi Mi",["Category2"],"Long descriptions","imagefile",450),
        };
}
