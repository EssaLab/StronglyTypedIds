using EssaLab.StronglyTypedIds;

namespace TestApp.Domain;

[StronglyTypedId<Guid>]
public partial record struct OrderId;

[StronglyTypedId<int>]
public partial record struct CustomerId;

[StronglyTypedId<string>]
public partial record struct CardId;

// [StronglyTypedId(IdType.Long)]
public partial record ItemId;

// [StronglyTypedId(IdType.Long)]
public partial record MohammedId();