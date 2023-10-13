namespace EssentialFrame.Specifications;

public static class SpecificationExtensions
{
    public static OrSpecification<T> Or<T>(this Specification<T> left, Specification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }
}