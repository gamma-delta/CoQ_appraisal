(defn- power [name class cost attr minimum snippet description]
  [:power {:Name name
           :Class class
           :Cost cost
           :Attribute attr
           :Minimum minimum
           :Snippet snippet
           :Description description}])

(defn skills []
  [:skills
   [:skill {:Name "Appraisal" :Class :PKAPP_AppraisalSkillTree
            :Description "You are skilled at visually identifying certain qualities of items and beings."
            :Cost 50 :Attribute "Intelligence" :Minimum 15
            :Snippet "appraisal"}
    (power "Scaler" :PKAPP_Mass
           0 :Intelligence 15
           "judging mass"
           "You can judge the mass of an object. (This includes its equipment.)")
    (power "Thermophist" :PKAPP_Thermometer
           50 :Intelligence 15
           "judging temperature"
           "You can judge the temperature of an object.")
    (power "Genetic Perspicacity" :PKAPP_MutantSpotter
           100 :Intelligence 17
           "divining genetic drift"
           "You can identify a creature's mutations, if any.")
    (power "Cybernetic Perspicacity" :PKAPP_CyberneticSpotter
           100 :Intelligence 17
           "winnowing the sight of chrome from flesh"
           "You can identify a creature's cybernetics, if any.")
    (power "Mercantility" :PKAPP_Price
           150 :Intelligence 19
           "appraising price"
           "You can judge the price of an object.")

    (power "Transparent Eye-ball" :PKAPP_TransparentEyeBall
           200 :Intelligence 21
           "bathing the head in the blithe air"
           "You identify exact information instead of generalities from your other Appraisal skills.")]])
