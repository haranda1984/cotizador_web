import React, { useEffect, useState } from "react";
import axios from "axios";
import endpoints from "../../api-service/endpoints";

const Unit = (props) => {
  const [unit, setunit] = useState([]);

  var getUnit = async () => {
    const { data } = await axios.get(`${endpoints.UNIT}/${props.id}`);
    setunit(data);
  };

  useEffect(() => {
    getUnit();
  }, []);

  return (
    <>
      <div>{unit.displayName}</div>
      <div>{unit.price}</div>
      <div>{unit.minimumExpectedValue}</div>
      <div>{unit.number}</div>
      <div>{unit.level}</div>
      <div>{unit.grossArea}</div>
      <div>{unit.status}</div>
    </>
  );
};

export default Unit;
