using EssaLab.StronglyTypedIds;

namespace TestApp.Models
{
    [StronglyTypedId<Guid>]
    public partial record OrderId;

};

namespace TestApp.Domain
{

    [StronglyTypedId<Guid>]
    public partial record OrderId;

    [StronglyTypedId<int>]
    public partial record struct CustomerId;

    [StronglyTypedId<string>]
    public partial record struct CardId;

    [StronglyTypedId<string>]
    public partial record struct ItemId;

[StronglyTypedId<int>]
    public partial record struct MohammedId;
}