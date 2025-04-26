namespace PlanetTweaks.Utils;

public static class PlanetUtils {
    public static scrPlanet GetThirdPlanet() {
        if(!scrController.instance)
            return null;
        foreach(scrPlanet planet in scrController.instance.planetarySystem.planetList)
            if(planet != scrController.instance.planetRed
               && planet != scrController.instance.planetBlue)
                return planet;
        return scrController.instance.planetarySystem.allPlanets[2];
    }
}