import React, { useEffect, useState } from "react";
import axios from "axios";
import endpoints from "../../api-service/endpoints";

const TradePolicy = (props) => {
  const [tradepolicy, settradepolicy] = useState([]);

  var getTradePolicy = async () => {
    const { data } = await axios.get(`${endpoints.TRADEPOLICY}/${props.id}`);
    settradepolicy(data);
  };

  useEffect(() => {
    getTradePolicy();    
  }, []);

  return (
    <>
      <div>{tradepolicy.name}</div>
      <div>{tradepolicy.discount}</div>
      <div>{tradepolicy.deposit}</div>
      <div>{tradepolicy.lastPayment}</div>
      <div>{tradepolicy.additionalPayments}</div>
      <div>{tradepolicy.monthlyPayments}</div>
    </>
  );
};

export default TradePolicy;
