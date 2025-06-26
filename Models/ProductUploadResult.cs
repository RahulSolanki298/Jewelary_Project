namespace Models
{
    public class ProductUploadResult
    {
        public bool IsSuccess { get; set; }
        public int NoOfProducts { get; set; } = 0;
        public int NoOfStyles { get; set; } = 0;
        public int NoOfCollections { get; set; } = 0;
        public string Message { get; set; }
        public string Errors { get; set; }
    }
}
