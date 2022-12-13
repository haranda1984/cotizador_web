import React, { useEffect, useState } from 'react'
import { Formik, Field, Form } from 'formik'
import axios from 'axios'
import { makeStyles } from '@material-ui/core/styles'
import * as Yup from 'yup'

import Modal from '@material-ui/core/Modal'
import Table from '@material-ui/core/Table'
import TableBody from '@material-ui/core/TableBody'
import TableCell from '@material-ui/core/TableCell'
import TableContainer from '@material-ui/core/TableContainer'
import TableHead from '@material-ui/core/TableHead'
import TableRow from '@material-ui/core/TableRow'
import IconButton from '@material-ui/core/IconButton'
import DeleteIcon from '@material-ui/icons/Delete'
import VpnKeyIcon from '@material-ui/icons/VpnKey';
import AddPhotoAlternateIcon from "@material-ui/icons/AddPhotoAlternate";
import Autocomplete from '@material-ui/lab/Autocomplete'
import TextField from '@material-ui/core/TextField'
import Snackbar from '@material-ui/core/Snackbar'
import MuiAlert from '@material-ui/lab/Alert'
import { TextField as FormikTextField } from 'formik-material-ui'

import Paper from '@material-ui/core/Paper'
import endpoints from '../../api-service/endpoints'
import Loader from 'react-loader-spinner'
import { PRIMARYCOLOR } from '../../constants/utils'
import { InverlevyButton } from '../InvyButton/InvyButton.component'
import styled from 'styled-components'

const UpdatePasswordSchema = Yup.object().shape({
  Password: Yup.string()
    .required('Requerido')
    .matches(
      /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$/,
      'La contraseña debe tener mínimo 8 caracteres, 1 carácter especial ( @$!%*#?& ) y 1 número'
    )
})

const StyledForm = styled(Form)`
  display: grid;
  grid-gap: 1em;
  width: 20em;
  align-items:center;
`

function Alert (props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />
}

const useStyles = makeStyles((theme) => ({
  table: {
    minWidth: 650
  },
  paper: {
    position: 'absolute',
    backgroundColor: theme.palette.background.paper,
    border: '2px solid #000',
    boxShadow: theme.shadows[5],
    padding: theme.spacing(2, 4, 3)
  }
}))

function getModalStyle () {
  const top = 50
  const left = 50

  return {
    top: `${top}%`,
    left: `${left}%`,
    transform: `translate(-${top}%, -${left}%)`,
    outline: 0
  }
}

const UserList = () => {
  const [users, setusers] = useState([])
  const [roles, setroles] = useState([])
  const [updateUser, setUpdateUser] = useState('')
  const [error, setError] = useState(false)
  const [success, setSuccess] = useState(false)
  const [open, setopen] = useState(false)
  const [isLoading, setIsLoading] = useState(true)
  const [modalStyle] = useState(getModalStyle)

  const classes = useStyles()

  const getUsers = async () => {
    const { data } = await axios.get(endpoints.USER)
    setusers(data.filter(x => x.isActive !== false))
  }

  const getRoles = async () => {
    const { data } = await axios.get(endpoints.ROLE)
    setroles(data)
  }

  const updateRoles = async (id, newRoles) => {
    await axios.post(`${endpoints.USER}/${id}/role`, newRoles.map(x => ({ Name: x })))
    setSuccess(true)
  }

  const deleteUser = async (id) => {
    await axios.delete(`${endpoints.USER}/${id}`)
    setSuccess(true)
    getUsers()
    setIsLoading(false)
  }
  const handleClose = () => setopen(false)
  const handleAlertClose = () => setError(false)
  const handleSuccessClose = () => setSuccess(false)
  const  handleUploadClick = async ( event , id) => {
    var file = event.target.files[0];
    const formData = new FormData();
    formData.append('file',file)
      try {
        await axios.post(`${endpoints.USER}/${id}/signature`,formData,{
          headers: {'content-type': 'multipart/form-data'  }
        })
        setSuccess(true)
      } catch (error) {
        setError(true)
      }
    
  }

  useEffect(() => {
    getRoles()
    getUsers()
    setIsLoading(false)
  }, [])

  if (isLoading) {
    return <Paper> <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}> <Loader type="ThreeDots" color={PRIMARYCOLOR}/> </div></Paper>
  }

  const body = (
    <div style={modalStyle} className={classes.paper}>
      <h2 id="simple-modal-title">Cambiar contraseña</h2>
      <Formik
        initialValues={{
          Password: ''
        }}
        validationSchema={UpdatePasswordSchema}
        onSubmit={async (values, actions) => {
          try {
            await axios.post(`${endpoints.ACCOUNT}/${updateUser}/changePassword`, {
              newPassword: values.Password
            })
            actions.resetForm({
              Password: ''
            })
            handleClose()
            setSuccess(true)
          } catch (error) {
            console.log(error)
            setError(true)
          }
        }}
      >{({ errors, isSubmitting }) => (
          <StyledForm>
            <Field
              component={FormikTextField}
              name="Password"
              variant="outlined"
              type="text"
              label="Contraseña"
              InputProps={{
                autoComplete: 'off'
              }}
            />
            { isSubmitting
              ? <Loader style={{ display: 'flex', justifyContent: 'center' }} type="ThreeDots" color={PRIMARYCOLOR}/>
              : <InvyButton type="submit" disabled={isSubmitting}> Cambiar contraseña</InvyButton>}
          </StyledForm>
        )}
      </Formik>
    </div>
  )

  return (
    <>
      <Snackbar open={error} autoHideDuration={10000} onClose={handleAlertClose}>
        <Alert onClose={handleAlertClose} severity="error">
          Ocurrió un error, por favor intente de nuevo. Si el error persiste contacte <a href="mailto:soporte@rootinc.mx">soporte@rootinc.mx </a>
        </Alert>
      </Snackbar>
      <Snackbar open={success} autoHideDuration={5000} onClose={handleSuccessClose}>
        <Alert onClose={handleSuccessClose} severity="success">
          Operación realizada con éxito.
        </Alert>
      </Snackbar>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="simple-modal-title"
        aria-describedby="simple-modal-description"
      >
        {body}
      </Modal>
      <TableContainer component={Paper}>
        <Table className={classes.table} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>Nombre</TableCell>
              <TableCell align="left">Apellidos</TableCell>
              <TableCell align="left">Email</TableCell>
              <TableCell align="left">Roles</TableCell>
              <TableCell align="right">Borrar</TableCell>
              <TableCell align="center">Cambiar contraseña</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((row) => (
              <TableRow key={row.id}>
                <TableCell component="th" scope="row">
                  {row.firstName}
                </TableCell>
                <TableCell align="left">{row.lastName}</TableCell>
                <TableCell align="left">{row.email}</TableCell>
                <TableCell align="right" style={{ maxWidth: 150 }} >
                  <Autocomplete
                    multiple
                    options={roles.map(x => x.name)}
                    getOptionLabel={(option) => option}
                    onChange={(e, value) => updateRoles(row.id, value)}
                    defaultValue={row.roles}
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        variant="standard"
                        label="Roles"
                        placeholder="Roles"
                      />
                    )}
                  /></TableCell>
                <TableCell align="right" >
                  <IconButton size="small" aria-label="delete" onClick={() => { setIsLoading(true); deleteUser(row.id) }}>
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
                <TableCell align="center" >
                  <IconButton size="small" aria-label="change password" onClick={() => { setUpdateUser(row.id); setopen(true) }}>
                    <VpnKeyIcon/>
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  )
}

export default UserList
