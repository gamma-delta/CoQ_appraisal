(use cbt/xml-helpers/objects)

(defn object-blueprints []
  [:objects
   (alter-object :PhysicalObject
                 (part :PKAPP_AppraisableHook))])
