import React from 'react'
import PropTypes from 'prop-types'

import Tabs from '@material-ui/core/Tabs'
import Tab from '@material-ui/core/Tab'
import Box from '@material-ui/core/Box'

import NewUser from '../../../components/NewUser/NewUser.component'
import UserList from '../../../components/UserList/UserList.component'
import ProjectList from '../../../components/ProjectList/ProjectList.component'

const TabPanel = (props) => {
  const { children, value, index, ...other } = props

  return (
    <div
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box p={3}>
          {children}
        </Box>
      )}
    </div>
  )
}

TabPanel.propTypes = {
  children: PropTypes.node,
  index: PropTypes.any.isRequired,
  value: PropTypes.any.isRequired
}

const Landing = () => {
  const [state, setstate] = React.useState(0)   //Pagina de inicio al cambiar a la seccion "ADMIN"

  const handleChange = (event, newValue) => {
    setstate(newValue)
  }

  return <div>
    <Tabs style={{ backgroundColor: 'white' }} value={state} onChange={handleChange} >
      <Tab label="Nuevo usuario" />
      <Tab label="Listado de usuarios" />
      <Tab label="Listado de proyectos" />
    </Tabs>

    <TabPanel value={state} index={0}>    
      <NewUser />
    </TabPanel>
    <TabPanel value={state} index={1}>
      <UserList />
    </TabPanel>
    <TabPanel value={state} index={2}>
      <ProjectList />
    </TabPanel>
  </div>
}

export default Landing
