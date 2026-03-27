namespace LexiFlow.Domain.Enums;

/// <summary>
/// Kelimenin zorluk seviyesi. Admin veya kullanıcı tarafından belirlenir.
/// Analytics raporlarında başarı oranları bu seviyeye göre gruplandırılabilir.
/// </summary>
public enum DifficultyLevel
{
    VeryEasy = 1,
    Easy     = 2,
    Medium   = 3,
    Hard     = 4,
    VeryHard = 5
}
