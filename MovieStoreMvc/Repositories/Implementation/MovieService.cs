using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext ctx;

        public MovieService(DatabaseContext _ctx) 
        {
            this.ctx = _ctx;
        }
        public bool Add(Movie model)
        {
            try
            {
                ctx.Movies.Add(model);
                ctx.SaveChanges();

                foreach (int genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    ctx.MovieGenres.Add(movieGenre);
                }
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if(data == null)
                {
                    return false;
                }

                var movieGenres = ctx.MovieGenres.Where(a => a.MovieId==data.Id);
                foreach(var movieGenre in movieGenres)
                {
                    ctx.MovieGenres.Remove(movieGenre);
                }

                ctx.Movies.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Movie GetById(int id)
        {
            return ctx.Movies.Find(id);
        }

        public MovieListVm List()
        {
            var list = ctx.Movies.ToList();
            foreach (var movie in list)
            {
                var genres = (from genre in ctx.Genres 
                              join mg in ctx.MovieGenres
                              on genre.Id equals mg.GenreId
                              where mg.MovieId==movie.Id
                              select genre.GenreName
                              ).ToList();
                var genreNames = string.Join(", ", genres);
                movie.GenreNames = genreNames;
            }
            var data = new MovieListVm
            {
                MovieList = list.AsQueryable()
            };
            return data;
        }

        public bool Update(Movie model)
        {
            try
            {
                //these genreIds are not selected by users and still present is movieGenre table corresponding to
                //this movieId. So these ids should be removed.
                var genresToDeleted = ctx.MovieGenres.Where(a => a.MovieId == model.Id 
                && !model.Genres.Contains(a.GenreId)).ToList();
                foreach(var mGenre in genresToDeleted) 
                {
                    ctx.MovieGenres.Remove(mGenre);
                }

                foreach (int genreId in model.Genres)
                {
                    var movieGenre = ctx.MovieGenres.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genreId);
                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genreId, MovieId = model.Id };
                        ctx.MovieGenres.Add(movieGenre);
                    }
                }

                ctx.Movies.Update(model);

                //we have to add these genre ids in movieGenre table

                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<int> GetGenreByMovieId(int movieId)
        {
            var genIds = ctx.MovieGenres.Where(a=>a.MovieId==movieId)
                .Select(a=>a.GenreId).ToList();
            return genIds;
        }
    }
}
