I recently need some code for Bezier curves and while there was a lot a stuff spread around the internet about curves it was difficult to find a single implementation that included all the commonly needed algorithms. I ended up writing a implementation based on a few sources (mostly this [one](https://www.codeproject.com/Articles/25237/Bezier-Curves-Made-Simple)) and thought I would shared the results as there were a few things that are quite hard to work out.

See [home page](https://www.digital-dust.com/single-post/2018/03/09/Bezier-Curves-in-Unity) for Unity package download.

I ended up with 3 Bezier curve classes.

First a general Bezier curve class based on a  Bernstein polynomial and able to support any degree of curve (Its capped at 31 due to precision issues). You would not normally use more than a cubic curve (degree 3)  however.

This class contains functions to find the position, normal, tangent or first derivative as well as find the total length of the curve via integration. There's also a function to split the curve at any position and return two new curves of the same degree.

Next is a Parametric Bezier class that extends the previous one and allows the the exact parameter at any length on the curve to be determined. This is useful to move along the curve at a constant speed or to create line segments from the curve that are evenly spaced.

Next is a Quadratic Bezier class. While the general Bezier class can represent a quadratic curve its use full to have a dedicated class for this degree of curve as it one of the most commonly used and many of its properties have closed form solutions.

This class contains functions to find the position, normal, tangent or first derivative as well as find the total length of the curve but with a closed for solution rather than integration. There's also a closed form solution to find the closest point on the curve to another point and if the curve intersects a segment.

Below is a image of linear, quadratic, cubic, quartic and quintic degree curve.

![Bezier Curves](https://static.wixstatic.com/media/1e04d5_fc3781020dc04a538216b06649c12946~mv2.png/v1/fill/w_550,h_550,al_c,usm_0.66_1.00_0.01/1e04d5_fc3781020dc04a538216b06649c12946~mv2.png)

And here's a comparison of the parametric curve (on the right) to a normal curve. Notice its segments are more evenly spaced.

![Bezier Curves](https://static.wixstatic.com/media/1e04d5_944e3646b21f4b438c6a4e639760a4ca~mv2.png/v1/fill/w_486,h_486,al_c,usm_0.66_1.00_0.01/1e04d5_944e3646b21f4b438c6a4e639760a4ca~mv2.png)
