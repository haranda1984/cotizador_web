import React, { useEffect, useReducer } from "react";
import { InverlevyButton } from "../../components/InvyButton/InvyButton.component";
import { makeStyles } from "@material-ui/core/styles";
import axios from "axios";
import TextField from "@material-ui/core/TextField";
import endpoints from "../../api-service/endpoints";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Loader from "react-loader-spinner";
import { MONEYFORMATTER, PRIMARYCOLOR } from "../../constants/utils";
import Paper from "@material-ui/core/Paper";
import SaveIcon from "@material-ui/icons/Save";
import AddIcon from "@material-ui/icons/Add";
import DeleteIcon from "@material-ui/icons/DeleteForever";
import PercentileIcon from "@material-ui/icons/TrafficSharp";
import Snackbar from "@material-ui/core/Snackbar";
import MuiAlert from "@material-ui/lab/Alert";

function Alert(props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />;
}

const useStyles = makeStyles((theme) => ({
  table: { minWidth: 650 },
  paper: {
    position: "absolute",
    backgroundColor: theme.palette.background.paper,
    border: "2px solid #000",
    boxShadow: theme.shadows[5],
    padding: theme.spacing(2, 4, 3),
  },
}));

const initialState = {
  tradepolicies: [],
  isLoading: true,
  success: false,
  error: false,
  sampleUnit: [],
  discount: 0.15,
};

const reducer = (state, action) => {
  switch (action.type) {
    case "SET_LOADING": {
      const { payload } = action;
      return { ...state, isLoading: payload };
    }
    case "SET_TRADEPOLICIES": {
      const { payload } = action;
      return { ...state, tradepolicies: payload };
    }
    case "SET_SUCCESS": {
      const { payload } = action;
      return { ...state, success: payload };
    }
    case "SET_ERROR": {
      const { payload } = action;
      return { ...state, error: payload };
    }
    case "SET_SAMPLEUNIT": {
      const { payload } = action;
      return { ...state, sampleUnit: payload };
    }
    case "SET_DISCOUNT": {
      const { payload } = action;
      return { ...state, discount: payload };
    }
    case "SET_POLICIES_LOADING": {
      const { payload } = action;
      return { ...state, ...payload };
    }
    default:
      throw new Error();
  }
};

const TradePolicyList = (props) => {
  const [state, dispatch] = useReducer(reducer, initialState);
  const classes = useStyles();

  const getUnit = async () => {
    const { data } = await axios.get(`${endpoints.PROJECT}/${props.id}/units`);
    let units = data.filter((tp) => tp.isActive) ?? [];
    let unit = units[0];
    dispatch({ type: "SET_SAMPLEUNIT", payload: unit });
    return unit;
  };

  const getTradePolicies = async () => {
    const { data } = await axios.get(
      `${endpoints.PROJECT}/${props.id}/tradepolicies`
    );

    await calcValues(data);
    dispatch({
      type: "SET_POLICIES_LOADING",
      payload: { isLoading: false, tradepolicies: data },
    });
  };

  const calcValues = async (originalData) => {
    const unit = await getUnit();
    const data = originalData ? originalData : [...state.tradepolicies];
    for (let i = 0; i < data.length; i++) {
      let finalPrice =
        unit.minimumExpectedValue -
        unit.minimumExpectedValue * Number(data[i].discount);
      data[i].finalPrice = finalPrice;
      data[i].vpn =
        finalPrice / Math.pow(1 + state.discount / 12, data[i].monthlyPayments);
    }

    let reformattedArray = data.map((o) => {
      return { id: o.id, vpn: o.vpn };
    });
    let r = reformattedArray.sort((a, b) => {
      return a.vpn - b.vpn;
    });
    for (let i = 0; i < data.length; i++) {
      let lessThan = r.filter((item) => item.vpn < data[i].vpn).length;
      let equalThan = r.filter((item) => item.vpn === data[i].vpn).length;
      let percentile = (100 * (lessThan + equalThan / 2)) / data.length;
      data[i].percentile = Math.round(percentile);
    }

    dispatch({
      type: "SET_POLICIES_LOADING",
      payload: { isLoading: false, tradepolicies: data },
    });
  };

  const updateDiscount = async (e) => {
    let newDiscount = e.target.value;
    dispatch({ type: "SET_DISCOUNT", payload: newDiscount });
    calcValues();
  };

  const updatePolicy = async (id, e) => {
    let newArr = [...state.tradepolicies];
    let index = newArr.findIndex((i) => i.id === id);
    newArr[index][e.target.name] = e.target.value;
    dispatch({ type: "SET_TRADEPOLICIES", payload: newArr });
    calcValues();
  };

  const addPolicy = async () => {
    let newArr = [...state.tradepolicies];
    newArr.push({
      projectId: props.id,
      name: "",
      discount: 0,
      deposit: 0,
      lastPayment: 0,
      additionalPayments: 0,
      monthlyPayments: 0,
      isActive: true,
    });
    dispatch({ type: "SET_TRADEPOLICIES", payload: newArr });
  };

  const persistPolicyUpdate = async (id) => {
    try {
      let index = state.tradepolicies.findIndex((i) => i.id === id); // Get the index of the tradepolicy to modify
      let policy = state.tradepolicies[index];
      if (policy.name === "") {
        Alert("Input is Invalid. Name can't be empty.");
        throw new Error("Input is Invalid. Name can't be empty.");
      }
      if (id !== undefined) {
        await axios.put(`${endpoints.TRADEPOLICY}/${id}`, {
          projectId: policy.projectId,
          name: policy.name,
          discount: Number(policy.discount),
          deposit: Number(policy.deposit),
          lastPayment: Number(policy.lastPayment),
          additionalPayments: Number(policy.additionalPayments),
          monthlyPayments: Number(policy.monthlyPayments),
          isActive: policy.isActive,
        }); // Update value in db thru api
      } else {
        await axios.post(`${endpoints.TRADEPOLICY}`, {
          projectId: props.id,
          name: policy.name,
          discount: Number(policy.discount),
          deposit: Number(policy.deposit),
          lastPayment: Number(policy.lastPayment),
          additionalPayments: Number(policy.additionalPayments),
          monthlyPayments: Number(policy.monthlyPayments),
          isActive: policy.isActive,
        }); // Create new value in db thru api
      }
      dispatch({ type: "SET_SUCCESS", payload: true });
    } catch (error) {
      dispatch({ type: "SET_ERROR", payload: true });
    }
  };

  const enableAndDisablePolicy = async (id) => {
    try {
      let newArr = [...state.tradepolicies]; // Copying the old tradepolicies array
      let index = state.tradepolicies.findIndex((i) => i.id === id); // Get the index of the tradepolicy to modify
      // First, update the state...
      newArr[index].isActive = !newArr[index].isActive;
      dispatch({ type: "SET_TRADEPOLICIES", payload: newArr });
      // ...Second, persist changes in the db via API
      if (newArr[index].isActive) {
        persistPolicyUpdate(id);
      } else {
        await axios.delete(`${endpoints.TRADEPOLICY}/${id}`); // Soft-Delete Policy in db thru api
      }
      dispatch({ type: "SET_SUCCESS", payload: true });
    } catch (error) {
      dispatch({ type: "SET_ERROR", payload: true });
    }
  };

  const handleAlertClose = () =>
    dispatch({ type: "SET_ERROR", payload: false });
  const handleSuccessClose = () =>
    dispatch({ type: "SET_SUCCESS", payload: false });

  const getStopLightColor = (row) => {
    if (row.percentile <= 10) return "#FF0000";
    if (row.percentile <= 20.0) return "#CC002E";
    if (row.percentile <= 30.0) return "#DD332E";
    if (row.percentile <= 40.0) return "#FF5500";
    if (row.percentile <= 50.0) return "#FFAA00";
    if (row.percentile <= 60.0) return "#FFFF2E";
    if (row.percentile <= 70.0) return "#D2FF2E";
    if (row.percentile <= 80.0) return "#9AFF2E";
    if (row.percentile <= 90.0) return "#62FF2E";

    return "#00FF00";
  };

  useEffect(() => {
    getTradePolicies();
  }, []);

  if (state.isLoading) {
    return (
      <Paper>
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Loader type="ThreeDots" color={PRIMARYCOLOR} />
        </div>
      </Paper>
    );
  }

  return (
    <>
      <Snackbar
        open={state.error}
        autoHideDuration={10000}
        onClose={handleAlertClose}
      >
        <Alert onClose={handleAlertClose} severity="error">
          Ocurrió un error, por favor intente de nuevo. Si el error persiste
          contacte
          <a href="mailto:soporte@rootinc.mx">soporte@rootinc.mx </a>
        </Alert>
      </Snackbar>
      <Snackbar
        open={state.success}
        autoHideDuration={5000}
        onClose={handleSuccessClose}
      >
        <Alert onClose={handleSuccessClose} severity="success">
          Operación realizada con éxito.
        </Alert>
      </Snackbar>
      <TableContainer component={Paper}>
        <Table className={classes.table} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>Unidad de ejemplo</TableCell>
              <TableCell>Nombre: </TableCell>
              <TableCell>
                {state.sampleUnit.name} {state.sampleUnit.number}
              </TableCell>
              <TableCell align="right" colSpan={2}>
                Precio Esperado:
              </TableCell>
              <TableCell>
                {MONEYFORMATTER.format(state.sampleUnit.minimumExpectedValue)}
              </TableCell>
              <TableCell></TableCell>
              <TableCell align="right">Descuento:</TableCell>
              <TableCell>
                <TextField
                  type="number"
                  name="discount"
                  value={state.discount}
                  variant="outlined"
                  id={`discount`}
                  onChange={(e) => updateDiscount(e)}
                  inputProps={{ className: "no-spin", max: 100 }}
                  style={{ width: "60px" }}
                />
              </TableCell>
              <TableCell align="right">
                <InverlevyButton
                  align="right"
                  backgroundColor={PRIMARYCOLOR}
                  borderColor={PRIMARYCOLOR}
                  onClick={() => addPolicy()}
                >
                  <AddIcon />
                  Agregar
                </InverlevyButton>
              </TableCell>
            </TableRow>

            <TableRow>
              <TableCell align="left">Nombre</TableCell>
              <TableCell align="center">Enganche</TableCell>
              <TableCell align="center">Construcción</TableCell>
              <TableCell align="center">Escrituración</TableCell>
              <TableCell align="center">Plazo</TableCell>
              <TableCell align="center">Descuento</TableCell>
              {/* <TableCell align="center">Precio Final</TableCell> */}
              <TableCell align="center">VPN</TableCell>
              <TableCell align="center">Percentil</TableCell>
              <TableCell align="center">Guardar</TableCell>
              <TableCell align="center">Eliminar</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {state.tradepolicies.map((row) => (
              <TableRow key={row.id}>
                <TableCell align="left">
                  <TextField
                    type="text"
                    name="name"
                    value={row.name}
                    variant="outlined"
                    id={`name_${row.id}`}
                    onChange={(e) => updatePolicy(row.id, e)}
                    inputProps={{ className: "no-spin", max: 100 }}
                    style={{ width: "300px" }}
                  />
                </TableCell>
                <TableCell align="center">
                  <TextField
                    type="number"
                    name="deposit"
                    value={row.deposit}
                    variant="outlined"
                    id={`deposit_${row.id}`}
                    onChange={(e) => updatePolicy(row.id, e)}
                    inputProps={{ className: "no-spin", max: 100 }}
                    style={{ width: "60px" }}
                  />
                </TableCell>
                <TableCell align="center">
                  <TextField
                    type="number"
                    name="additionalPayments"
                    value={row.additionalPayments}
                    variant="outlined"
                    id={`additionalPayments_${row.id}`}
                    onChange={(e) => updatePolicy(row.id, e)}
                    inputProps={{ className: "no-spin", max: 100 }}
                    style={{ width: "60px" }}
                  />
                </TableCell>
                <TableCell align="center">
                  <TextField
                    type="number"
                    name="lastPayment"
                    value={row.lastPayment}
                    variant="outlined"
                    id={`lastPayment_${row.id}`}
                    onChange={(e) => updatePolicy(row.id, e)}
                    inputProps={{ className: "no-spin", max: 100 }}
                    style={{ width: "60px" }}
                  />
                </TableCell>
                <TableCell align="center">
                  <TextField
                    type="number"
                    name="monthlyPayments"
                    value={row.monthlyPayments}
                    variant="outlined"
                    id={`monthlyPayments_${row.id}`}
                    onChange={(e) => updatePolicy(row.id, e)}
                    inputProps={{ className: "no-spin", max: 100 }}
                    style={{ width: "60px" }}
                  />
                </TableCell>
                <TableCell align="center">
                  <TextField
                    type="number"
                    name="discount"
                    value={row.discount}
                    variant="outlined"
                    id={`discount_${row.id}`}
                    onChange={(e) => updatePolicy(row.id, e)}
                    inputProps={{ className: "no-spin", max: 100 }}
                    style={{ width: "60px" }}
                  />
                </TableCell>
                <TableCell align="center">
                  {MONEYFORMATTER.format(row.vpn)}
                </TableCell>
                <TableCell align="center">
                  <PercentileIcon htmlColor={getStopLightColor(row)} />
                </TableCell>
                <TableCell align="center">
                  <SaveIcon
                    style={{ cursor: "pointer" }}
                    htmlColor={"#a80e0e"}
                    onClick={() => persistPolicyUpdate(row.id)}
                  />
                </TableCell>
                <TableCell align="center">
                  <DeleteIcon
                    style={{ cursor: "pointer" }}
                    htmlColor={row.isActive ? PRIMARYCOLOR : "#767676"}
                    onClick={() => enableAndDisablePolicy(row.id)}
                  />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
};

export default TradePolicyList;
