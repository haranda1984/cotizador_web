import React, { useEffect, useState } from "react";
import axios from "axios";
import endpoints from "../../api-service/endpoints";

const UnitCount = props => {
    const [units, setunits] = useState([]);
    
    var getunits = async () => {
      const { data } = await axios.get(`${endpoints.PROJECT}/${props.id}/units`);
      setunits(data);
    };
  
    useEffect(() => {
      getunits();
    }, []);
    
    return (
      <div>{units.length}</div>
    );
  };

export default UnitCount;