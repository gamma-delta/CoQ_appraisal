(defn- append-skills [subtype & skills]
  [:subtype {:Name subtype :Load "Merge"}
   [:skills {:Load "Merge"}
    ;(map (fn [s]
            [:skill {:Name s}])
          skills)]])

(defn subtypes []
  [:subtypes
   [:class {:ID "Castes" :Load "Merge"}
    [:category {:Name "Ekuemekiyye" :Load "Merge"}
     # flavor: uh there isn't one, it's just that horti
     # is usually the TK recommended for the beginner
     (append-skills "Horticulturist" "PKAPP_MutantSpotter")]
    [:category {:Name "Yawningmoon" :Load "Merge"}
     (append-skills "Child of the Hearth" "PKAPP_Thermometer")]]

   [:class {:ID "Callings" :Load "Merge"}
    (append-skills "Nomad" "PKAPP_Mass")
    # flavor: wardens should know when a penis templar is rolling up
    (append-skills "Warden" "PKAPP_CyberneticSpotter")
    (append-skills "Water Merchant" "PKAPP_Price")]])
