using Unity.Mathematics;

public static class Math
{
    /// <summary>
     /// Изменяет размер вектора до границы с сохранением направления
     /// </summary>
     /// <param name="vector">Изменяемый вектор</param>
     /// <param name="vectorApplicationPoint">Точка приложения вектора внутри границ</param>
     /// <param name="boundPosition">Позиция границы</param>
     /// <param name="boundsExtents">Половина размера(диагонали) границы</param>
     /// <returns></returns>
    public static float3 BoundsClamp(
        in float3 vector,
        in float3 vectorApplicationPoint, 
        in float3 boundPosition, 
        in float3 boundsExtents
    )
    {
        //Смещение начала вектора в начало координат
        var offset = boundPosition - vectorApplicationPoint;
        var max = boundsExtents + offset;
        var min = -boundsExtents + offset;

        //Если точка не находится внутри границ возвращаем обычный кламп
        if (math.lengthsq(math.clamp(float3.zero, min, max)) > 0)
            return math.clamp(vector, min, max);
		
        var extentsLength = math.length(boundsExtents);
        var normalizedVector = math.normalize(vector);

        var expandedDirection = normalizedVector * extentsLength;
        var clampedDirection = math.clamp(expandedDirection, min, max);

        var clampError = clampedDirection - expandedDirection;
        var clampErrorLength = math.length(clampError);

        var cos = math.dot(-normalizedVector, clampError / clampErrorLength);
        var excess = clampErrorLength / cos;

        return normalizedVector * (extentsLength - excess);
    }
}