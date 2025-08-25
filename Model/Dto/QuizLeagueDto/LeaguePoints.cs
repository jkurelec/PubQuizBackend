using PubQuizBackend.Exceptions;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class LeaguePoints
    {
        public LeaguePoints()
        {
        }

        public LeaguePoints(int position, double points)
        {
            Position = position;
            Points = points;
        }

        public int Position { get; set; }
        public double Points { get; set; }

        public static IEnumerable<LeaguePoints> GetLeaguePointsList(string points)
        {
            var pointsList = new List<LeaguePoints>();
            try
            {
                var positionPoints = points.Split('|');

                foreach (var position in positionPoints)
                {
                    var result = position.Split("=");
                    pointsList.Add(
                        new LeaguePoints(
                            int.Parse(result[0]),
                            double.Parse(result[1])
                        )
                    );
                }
            }
            catch
            {
                throw new BadRequestException("Bad points format!");
            }

            return pointsList;
        }
    }
}
