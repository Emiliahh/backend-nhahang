namespace backend.Exceptions
{
    public class ProductException
    {
        // category is not exist
        public class CategoryNotExistException(string id):Exception($"category with {id} not exist");
        //product already exist 
        public class ProductAlreadyExistException(string id):Exception($"product with {id} is already exist");
        // product not found 
        public class ProductNotFoundException(string id):Exception($"product with with {id} not found");
    }
}
